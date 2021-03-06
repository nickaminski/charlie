using charlie.bll.interfaces;
using charlie.dto.Card;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        ICardSetProvider _cardSetProv;
        ICardProvider _cardProv;
        IDeckProvider _deckProv;
        ILogWriter _logger;
                                                                                                                                                          
        public CardController(ICardSetProvider cardSetProv, ICardProvider cardProv, IDeckProvider deckProv, ILogWriter logger)
        {
            _cardSetProv = cardSetProv;
            _cardProv = cardProv;
            _deckProv = deckProv;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CardSet>> GetSets([FromQuery]string maxYear)
        {
            _logger.ServerLogInfo("/Card/GetAllSets");
            return await _cardSetProv.GetSets(maxYear);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Card>> GetAllCardsInSet([FromQuery]string setName = "")
        {
            _logger.ServerLogInfo("/Card/GetAllCardsInSet");
            if (string.IsNullOrEmpty(setName))
                return await Task.FromResult(new List<Card>());

            return await _cardProv.GetAllCardsInSet(setName);
        }

        [HttpGet("[action]")]
        public async Task<Card> GetCardById([FromQuery]int id)
        {
            _logger.ServerLogInfo("/Card/GetCardById");
            return await _cardProv.GetCardById(id);
        }

        [HttpGet("[action]")]
        public async Task<Deck> GetDeckById([FromQuery]string guid)
        {
            _logger.ServerLogInfo("/Card/GetDeckById");
            return await _deckProv.GetDeckById(guid);
        }

        [HttpPost("[action]")] 
        public async Task<Deck> SaveDeck([FromBody]Deck deck)
        {
            _logger.ServerLogInfo("/Card/SaveDeck");
            return await _deckProv.SaveDeck(deck);
        }

        [HttpDelete("[action]")]
        public async Task<bool> DeleteDeck([FromQuery]string guid)
        {
            _logger.ServerLogInfo("/Card/DeleteDeck");
            return await _deckProv.DeleteDeck(guid);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Deck>> GetDecks()
        {
            _logger.ServerLogInfo("/Card/GetDecks");
            return await _deckProv.GetDecks();
        }

        [HttpGet("[action]")]
        public async Task<CardCollection> GetCollection()
        {
            _logger.ServerLogInfo("/Card/GetCollection");
            return await _cardProv.GetCollection();
        }

        [HttpDelete("[action]")]
        public IActionResult DeleteCollection()
        {
            _logger.ServerLogInfo("/Card/DeleteCollection");
            _cardProv.DeleteCollection();
            return Ok(true);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> AddToCollection([FromBody]List<Card> newCards)
        {
            _logger.ServerLogInfo("/Card/AddToCollection");
            await _cardProv.AddToCollection(newCards);
            return Ok(true);
        }

    }
}
