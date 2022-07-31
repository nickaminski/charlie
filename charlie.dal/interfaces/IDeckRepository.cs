using charlie.dto.Card;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IDeckRepository
    {
        Task<Deck> GetDeckById(Guid id);
        Task<Deck> SaveDeck(Deck deck);
        Task<bool> DeleteDeck(Guid id);
        Task<IEnumerable<Deck>> GetDecks();
    }
}
