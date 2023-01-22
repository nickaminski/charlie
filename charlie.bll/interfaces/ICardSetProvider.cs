using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICardSetProvider : IProvider<CardSet>
    {
        Task<IEnumerable<CardSet>> GetSets(string maxYear);
        Task<CardSet> GetSetByName(string name);
    }
}
