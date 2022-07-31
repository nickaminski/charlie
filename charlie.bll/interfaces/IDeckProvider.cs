using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IDeckProvider : IProvider<Deck>
    {
        Task<Deck> GetDeckById(string id);
        Task<Deck> SaveDeck(Deck deck);
        Task<bool> DeleteDeck(string id);
        Task<IEnumerable<Deck>> GetDecks();
    }
}
