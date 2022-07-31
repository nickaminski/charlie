using System.Collections.Generic;

namespace charlie.dto.Form
{
    public class FileQuestionContent : AnswerContent
    {
        public string file { get; set; }
        public IEnumerable<string> fileTypesAllowed { get; set; }
        public int maximumNumberFiles { get; set; }
        public string maximumFileSize { get; set; }
    }
}
