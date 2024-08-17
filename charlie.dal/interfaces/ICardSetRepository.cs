using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface ICardSetRepository
    {
        Task<IEnumerable<CardSet>> GetAllAsync();
        Task WriteCardSetDataAsync(string data);
    }
}
