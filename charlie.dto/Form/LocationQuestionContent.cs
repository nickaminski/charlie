namespace charlie.dto.Form
{
    public class LocationQuestionContent : AnswerContent
    {
        public double centerLat { get; set; }
        public double centerLon { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
    }
}
