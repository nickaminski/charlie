using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class CardProvider : ICardProvider
    {
        private ICardRepository _cardRepo;
        private IYGoProRepository _ygoRepo;
        private ILogWriter _logger;
        private IConfiguration _configuration;

        public CardProvider(ICardRepository cardSetRepo, IYGoProRepository ygoRepo, ILogWriter logger, IConfiguration configuration)
        {
            _cardRepo = cardSetRepo;
            _ygoRepo = ygoRepo;
            _logger = logger;
            _configuration = configuration;
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
                var path = _configuration["CardImagesPath"];
                var cardSetData = await _ygoRepo.GetAllCardsInSet(setName);

                if (!string.IsNullOrEmpty(cardSetData))
                {
                    List<Task> tasks = new List<Task>();
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardSetData);
                    Directory.CreateDirectory(Path.Combine(path, "cards"));
                    Directory.CreateDirectory(Path.Combine(path, "cards_small"));
                    foreach (var card in obj.data)
                    {
                        tasks.Add(_ygoRepo.DownloadImages(card.card_images, path));

                        foreach (var item in card.card_images)
                        {
                            item.image_url = string.Format("/cards/{0}.jpg", item.id);
                            item.image_url_small = string.Format("/cards_small/{0}.jpg", item.id);
                        }
                    }
                    var unwrappedData = JsonConvert.SerializeObject(obj.data);
                    tasks.Add(_cardRepo.WriteCardDataToSetFile(unwrappedData, setName));
                    await Task.WhenAll(tasks);
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
                var cardData = await _ygoRepo.GetCardById(id);

                if (!string.IsNullOrEmpty(cardData))
                {
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardData);
                    var card = obj.data.FirstOrDefault();
                    var path = _configuration["CardImagesPath"];
                    Directory.CreateDirectory(Path.Combine(path, "cards"));
                    Directory.CreateDirectory(Path.Combine(path, "cards_small"));

                    var task1 = _ygoRepo.DownloadImages(card.card_images, path);

                    foreach (var item in card.card_images)
                    {
                        item.image_url = string.Format("/cards/{0}.jpg", item.id);
                        item.image_url_small = string.Format("/cards_small/{0}.jpg", item.id);
                    }

                    var task2 = _cardRepo.WriteCardDataToAllFile(JsonConvert.SerializeObject(card));

                    await Task.WhenAll(task1, task2);

                    return card;
                }

                return new Card();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached card id: {0} from local", id);
                return results;
            }
        }

        public async Task<Card> GetCardByName(string name)
        {
            var results = await _cardRepo.GetByName(name);

            if (results == null)
            {
                _logger.ServerLogInfo("Fetching card name: {0} from YGoPro", name);
                var cardData = await _ygoRepo.GetCardByName(name);

                if (!string.IsNullOrEmpty(cardData))
                {
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardData);
                    var card = obj.data.FirstOrDefault();
                    var path = _configuration["CardImagesPath"];
                    Directory.CreateDirectory(Path.Combine(path, "cards"));
                    Directory.CreateDirectory(Path.Combine(path, "cards_small"));

                    var task1 = _ygoRepo.DownloadImages(card.card_images, path);

                    foreach (var item in card.card_images)
                    {
                        item.image_url = string.Format("/cards/{0}.jpg", item.id);
                        item.image_url_small = string.Format("/cards_small/{0}.jpg", item.id);
                    }

                    var task2 = _cardRepo.WriteCardDataToAllFile(JsonConvert.SerializeObject(card));

                    await Task.WhenAll(task1, task2);

                    return card;
                }

                return new Card();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached card name: {0} from local", name);
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
