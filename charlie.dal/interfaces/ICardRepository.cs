using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllCardsInSetAsync(string setName);
        Task WriteCardDataToSetFileAsync(string data, string setName);
        Task<Card> GetByIdAsync(int id);
        Task<Card> GetByNameAsync(string name);
        Task WriteCardDataToAllFileAsync(string data);
        Task<CardCollection> GetCollectionAsync();
        Task AddToCollectionAsync(IEnumerable<Card> newCards);
        void DeleteCollection();
    }
}
