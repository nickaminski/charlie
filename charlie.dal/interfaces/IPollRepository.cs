using charlie.dto.Poll;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IPollRepository
    {
        Task<string> CreatePollAsync(Poll poll);
        Task<IEnumerable<Poll>> GetAllAsync();
        Task<Poll> GetPollAsync(string id);
        Task<PollResults> GetPollResultsAsync(string id, string ipAddress);
        Task<bool> SavePollAsync(Poll poll);
    }
}
