using System.Collections.Generic;

namespace charlie.dto.Form
{
    public class GridQuestionContent : AnswerContent
    {
        public IEnumerable<Choice> rows { get; set; }
        public IEnumerable<Choice> columns { get; set; }
        public bool limitColumnResponse { get; set; }
        public bool shuffleRowOrder { get; set; }
    }
}
