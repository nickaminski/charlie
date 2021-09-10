using charlie.bll.interfaces;
using charlie.dto;

namespace charlie.bll.providers
{
    public class LoggerFormatter : ILoggerFormatter
    {
        public string Format(LoggingMessage message)
        {
            return string.Format("{0}--{1}--{2}--{3}--{4}", message.ClientIp, message.Source, message.Level, message.Message, message.Timestamp.ToLocalTime());
        }
    }
}
