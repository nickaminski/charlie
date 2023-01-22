using charlie.bll.interfaces;
using charlie.common.models;
using charlie.dto.Card;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        IConfiguration _configuration;
        ICardSetProvider _cardSetProv;
        ICardProvider _cardProv;
        IDeckProvider _deckProv;
        ILogWriter _logger;
        ICachingService _cachingService;
                                                                                                                                                          
        public CardController(ICardSetProvider cardSetProv, ICardProvider cardProv, IDeckProvider deckProv, ILogWriter logger, ICachingService cachingService, IConfiguration configuration)
        {
            _cardSetProv = cardSetProv;
            _cardProv = cardProv;
            _deckProv = deckProv;
            _logger = logger;
            _cachingService = cachingService;
            _configuration = configuration;
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
        public async Task<Card> GetCardByName([FromQuery]string name)
        {
            _logger.ServerLogInfo("/Card/GetCardByName");
            return await _cardProv.GetCardByName(name);
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

        [HttpGet("[action]")]
        public async Task<IActionResult> StartDraft([FromQuery]string setName = "", [FromQuery] int numPacks = 5)
        {
            if (string.IsNullOrEmpty(setName)) return BadRequest("setName must be something");

            var set = await _cardProv.GetAllCardsInSet(setName);
            if (set.Count() == 0)
                return BadRequest("no cards in set");

            var draftKey = Guid.NewGuid();
            var state = new DraftState() { draft_status = "in_progress", current_pack = 0, set_name = setName, num_packs = numPacks };
            _cachingService.Set(draftKey.ToString(), state);

            return Ok(draftKey);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> RollDraftPack([FromQuery]Guid draftKey)
        {
            var state = _cachingService.Get<DraftState>(draftKey.ToString());

            if (state == null || !state.draft_status.Equals("in_progress"))
                return BadRequest("no in progress draft to continue");

            var cards = await _cardProv.OpenPack(state.set_name);

            state.current_pack++;

            if (state.current_pack > state.num_packs)
            {
                _cachingService.Remove(draftKey.ToString());
            }
            else
            {
                _cachingService.Set(draftKey.ToString(), state);
            }

            return Ok(cards);
        }
    }
}
