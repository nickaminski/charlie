using charlie.dto.Card;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IDeckRepository
    {
        Task<Deck> GetDeckByIdAsync(Guid id);
        Task<Deck> SaveDeckAsync(Deck deck);
        Task<bool> DeleteDeckAsync(Guid id);
        Task<IEnumerable<Deck>> GetDecksAsync();
    }
}
