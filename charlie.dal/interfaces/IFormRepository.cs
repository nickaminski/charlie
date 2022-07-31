using charlie.dto.Form;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IFormRepository
    {
        Task<FormModel> SaveForm(FormModel form);
        Task<FormModel> GetForm(string id);
        Task<IEnumerable<FormModel>> GetAll();
    }
}
