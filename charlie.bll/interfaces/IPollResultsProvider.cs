using charlie.dto;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IPollResultsProvider : IProvider<PollResults>
    {
        Task<PollResults> GetPollResults(string id, string ipAddress); 
        Task<bool> SubmitResults(string id, string selectedChoice, string ipAddress);
    }
}
