using charlie.dto.Form;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IFormRepository
    {
        Task<FormModel> SaveFormAsync(FormModel form);
        Task<FormModel> GetFormAsync(string id);
        Task<IEnumerable<FormModel>> GetAllAsync();
    }
}
