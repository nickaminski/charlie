using System;

namespace charlie.dto
{
    public class LoggingMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Timestamp { get; set; }
        public int RetryCount { get; set; }
        public string Source { get; set; }
        public string ClientIp { get; set; }
    }
}
