using charlie.dto.Poll;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IPollProvider : IProvider<PollViewModel>
    {
        Task<string> CreatePoll(PollViewModel newPoll);
        Task<IEnumerable<PollViewModel>> GetAll(string clientIp);
        Task<PollViewModel> GetPoll(string id, string clientIp);
    }
}
