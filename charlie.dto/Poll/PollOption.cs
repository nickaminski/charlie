using System.Collections.Generic;

namespace charlie.dto.Poll
{
    public class PollOption
    {
        public string text { get; set; }
        public List<string> respondants { get; set; }
    }
}
