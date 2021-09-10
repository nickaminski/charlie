using charlie.common.models;
using System;
using System.Collections.Generic;

namespace charlie.dto
{
    public class Poll
    {
        public string id { get; set; }
        public string question { get; set; }
        public IEnumerable<PollOption> options { get; set; }
        public DateTime expirationDate { get; set; }
        public Time expirationTime { get; set; }
    }
}
