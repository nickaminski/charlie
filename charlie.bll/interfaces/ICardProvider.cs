using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICardProvider : IProvider<Card>
    {
        Task<IEnumerable<Card>> GetAllCardsInSet(string setName);
        Task<Card> GetCardById(int id);
        Task AddToCollection(IEnumerable<Card> newCards);
        Task<CardCollection> GetCollection();
        void DeleteCollection();
    }
}
