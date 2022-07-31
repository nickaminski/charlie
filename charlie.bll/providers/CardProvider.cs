using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Card;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class CardProvider : ICardProvider
    {
        private ICardRepository _cardRepo;
        private IYGoProRepository _ygoRepo;
        private ILogWriter _logger;

        public CardProvider(ICardRepository cardSetRepo, IYGoProRepository ygoRepo, ILogWriter logger)
        {
            _cardRepo = cardSetRepo;
            _ygoRepo = ygoRepo;
            _logger = logger;
        }

        public async Task AddToCollection(IEnumerable<Card> newCards)
        {
            _logger.ServerLogInfo("Adding {0} cards to collection", newCards.Count());
            await _cardRepo.AddToCollection(newCards);
        }

        public void DeleteCollection()
        {
            _logger.ServerLogInfo("Deleting collection");
            _cardRepo.DeleteCollection();
        }

        public async Task<IEnumerable<Card>> GetAllCardsInSet(string setName)
        {
            var results = await _cardRepo.GetAllCardsInSet(setName);

            if (results == null || results.Count() == 0)
            {
                _logger.ServerLogInfo("Fetching set: {0} from YGoPro", setName);
                var cardSetData = await _ygoRepo.GetAllCardsInSet(setName);

                if (!string.IsNullOrEmpty(cardSetData))
                {
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardSetData);
                    var unwrappedData = JsonConvert.SerializeObject(obj.data);
                    await _cardRepo.WriteCardDataToSetFile(unwrappedData, setName);
                    return obj.data;
                }

                return new List<Card>();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached set: {0} from local", setName);
                return results;
            }
        }

        public async Task<Card> GetCardById(int id)
        {
            var results = await _cardRepo.GetById(id);

            if (results == null)
            {
                _logger.ServerLogInfo("Fetching card id: {0} from YGoPro", id);
                var cardSetData = await _ygoRepo.GetCardById(id);

                if (!string.IsNullOrEmpty(cardSetData))
                {
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardSetData);
                    var unwrappedData = JsonConvert.SerializeObject(obj.data);
                    await _cardRepo.WriteCardDataToAllFile(unwrappedData);
                    return obj.data.FirstOrDefault();
                }

                return new Card();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached card id: {0} from local", id);
                return results;
            }
        }

        public async Task<CardCollection> GetCollection()
        {
            _logger.ServerLogInfo("Retrieving collection");
            return await _cardRepo.GetCollection();
        }
    }
}
