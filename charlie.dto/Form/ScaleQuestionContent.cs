namespace charlie.dto.Form
{
    public class ScaleQuestionContent : AnswerContent
    {
        public int minValue { get; set; }
        public int maxValue { get; set; }
        public string minLabel { get; set; }
        public string maxLabel { get; set; }
    }
}
