using charlie.bll.interfaces;
using charlie.dal;
using charlie.dal.interfaces;
using charlie.dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class DeckProvider : IDeckProvider
    {
        private IDeckRepository _repo;

        public DeckProvider(IDeckRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> DeleteDeck(string id)
        {
            if (Guid.TryParse(id, out var guid))
            {
                await _repo.DeleteDeck(guid);
                return false;
            }
            return false;
        }

        public async Task<Deck> GetDeckById(string id)
        {
            if (Guid.TryParse(id, out var guid))
            {
                return await _repo.GetDeckById(guid);
            }

            return null;
        }

        public async Task<Deck> SaveDeck(Deck deck)
        {
            return await _repo.SaveDeck(deck);
        }

        public async Task<IEnumerable<Deck>> GetDecks()
        {
            var decks = await _repo.GetDecks();
            foreach (var item in decks)
            {
                item.cards.Clear();
                item.cardCount.Clear();
            }
            return decks;
        }
    }
}
