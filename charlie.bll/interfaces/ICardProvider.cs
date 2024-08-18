using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICardProvider : IProvider<Card>
    {
        Task<IEnumerable<Card>> GetAllCardsInSet(string setName, CancellationToken token);
        Task<Card> GetCardById(int id, CancellationToken token);
        Task<Card> GetCardByName(string name, CancellationToken token);
        Task AddToCollection(IEnumerable<Card> newCards);
        Task<CardCollection> GetCollection();
        Task<IEnumerable<Card>> OpenPack(string setName, CancellationToken token);
        void DeleteCollection();
    }
}
