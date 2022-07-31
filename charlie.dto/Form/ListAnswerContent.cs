using System.Collections.Generic;

namespace charlie.dto.Form
{
    public class ListAnswerContent : AnswerContent
    {
        public IEnumerable<Choice> choices { get; set; }
        public bool shuffleOptionOrder { get; set; }
        public bool goToSection { get; set; }
        public bool responseValidation { get; set; }
    }
}
