using charlie.dto;

namespace charlie.bll.interfaces
{
    public interface ILoggerFormatter
    {
        string Format(LoggingMessage contents);
    }
}
