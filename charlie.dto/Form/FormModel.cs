using System.Collections.Generic;

namespace charlie.dto.Form
{
    public class FormModel
    {
        public IEnumerable<FormSection> sections { get; set; }
        public string formId { get; set; }
    }
}
