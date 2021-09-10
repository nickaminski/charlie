using System.Collections.Generic;

namespace charlie.dto
{
    public class PollResults
    {
        public string pollId { get; set; }
        public string question { get; set; }
        public string userChoice { get; set; }
        public Dictionary<string, int> results { get; set; }
    }
}
