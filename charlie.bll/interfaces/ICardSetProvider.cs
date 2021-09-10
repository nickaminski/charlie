using charlie.dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICardSetProvider : IProvider<CardSet>
    {
        Task<IEnumerable<CardSet>> GetSets(string maxYear);
    }
}
