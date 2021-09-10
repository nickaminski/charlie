using charlie.dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface IFormProvider : IProvider<FormModel>
    {
        public Task<IEnumerable<FormModel>> GetAll();
        public Task<FormModel> GetById(string id);
        public Task<string> CreateForm(FormModel newForm);
        public Task<bool> UpdateForm(FormModel data);
        public AnswerContent getAnswerContent(string questionType, string json);
    }
}
