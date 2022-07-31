using System.Collections.Generic;

namespace charlie.dto.Form
{
    public class FormSection
    {
        public string sectionId { get; set; }
        public string sectionTitle { get; set; }
        public string sectionDescription { get; set; }
        public IEnumerable<QuestionData> questions { get; set; }
    }
}
