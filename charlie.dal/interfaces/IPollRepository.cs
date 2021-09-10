using charlie.dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IPollRepository
    {
        Task<string> CreatePoll(Poll poll);
        Task<IEnumerable<Poll>> GetAll();
        Task<Poll> GetPoll(string id);
        Task<PollResults> GetPollResults(string id, string ipAddress);
        Task<bool> SavePoll(Poll poll);
    }
}
