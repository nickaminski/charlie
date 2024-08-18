using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICardSetProvider : IProvider<CardSet>
    {
        Task<IEnumerable<CardSet>> GetSets(string maxYear, CancellationToken token);
        Task<CardSet> GetSetByName(string name, CancellationToken token);
    }
}
