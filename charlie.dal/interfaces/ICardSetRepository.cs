using charlie.dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface ICardSetRepository
    {
        Task<IEnumerable<CardSet>> GetAll();
        Task WriteCardSetData(string data);
    }
}
