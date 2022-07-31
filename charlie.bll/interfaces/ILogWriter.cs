using charlie.dto;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ILogWriter
    {
        public Task ConsumeLogs(CancellationToken stoppingToken);
        public void AddMessage(LoggingMessage message);
        public void ServerLogInfo(string message);
        public void ServerLogWarning(string message);
        public void ServerLogError(string message);
        public void ServerLogInfo(string format, params object?[] args);
        public void ServerLogWarning(string format, params object?[] args);
        public void ServerLogError(string format, params object?[] args);
    }
}
