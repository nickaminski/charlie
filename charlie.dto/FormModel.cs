using System.Collections.Generic;

namespace charlie.dto
{
    public class FormModel
    {
        public IEnumerable<FormSection> sections { get; set; }
        public string formId { get; set; }
    }

    public class FormSection
    {
        public string sectionId { get; set; }
        public string sectionTitle { get; set; }
        public string sectionDescription { get; set; }
        public IEnumerable<QuestionData> questions { get; set; }
    }

    public class QuestionData
    {
        public string questionText { get; set; }
        public string questionType { get; set; }
        public bool required { get; set; }
        public string id { get; set; }
        public string jsonAnswerContent { get; set; }
        public AnswerContent answerContent { get; set; }
    }

    public class AnswerContent
    {
        public string description { get; set; }
    }

    public class ListAnswerContent : AnswerContent
    {
        public IEnumerable<Choice> choices { get; set; }
        public bool shuffleOptionOrder { get; set; }
        public bool goToSection { get; set; }
        public bool responseValidation { get; set; }
    }

    public class FileQuestionContent : AnswerContent
    {
        public string file { get; set; }
        public IEnumerable<string> fileTypesAllowed { get; set; }
        public int maximumNumberFiles { get; set; }
        public string maximumFileSize { get; set; }
    }

    public class ScaleQuestionContent : AnswerContent
    {
        public int minValue { get; set; }
        public int maxValue { get; set; }
        public string minLabel { get; set; }
        public string maxLabel { get; set; }
    }

    public class GridQuestionContent : AnswerContent
    {
        public IEnumerable<Choice> rows { get; set; }
        public IEnumerable<Choice> columns { get; set; }
        public bool limitColumnResponse { get; set; }
        public bool shuffleRowOrder { get; set; }
    }

    public class DateQuestionContent : AnswerContent
    {
        public bool includeTime { get; set; }
        public int includeYear { get; set; }
    }

    public class TimeQuestionContent : AnswerContent
    {
        public string answerType { get; set; }
        public bool timeDuration { get; set; }
    }

    public class LocationQuestionContent: AnswerContent
    {
        public double centerLat { get; set; }
        public double centerLon { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class TextQuestionContent : AnswerContent
    {
        public bool responseValidation { get; set; }
    }

    public class Choice
    {
        public string text { get; set; }
        public string image { get; set; }
        public int index { get; set; }
    }

}
