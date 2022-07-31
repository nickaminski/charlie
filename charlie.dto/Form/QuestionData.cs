namespace charlie.dto.Form
{
    public class QuestionData
    {
        public string questionText { get; set; }
        public string questionType { get; set; }
        public bool required { get; set; }
        public string id { get; set; }
        public string jsonAnswerContent { get; set; }
        public AnswerContent answerContent { get; set; }
    }
}
