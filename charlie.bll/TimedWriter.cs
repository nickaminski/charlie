using charlie.bll.interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using charlie.dto;
using System.Threading;

namespace charlie.bll
{
    public class TimedWriter : ILogWriter
    {
        private ILoggerFormatter _formatter;
        Queue<string> messagesToWriteA;
        Queue<string> messagesToWriteB;
        bool UseA;
        string logFilePath;

        public TimedWriter(IConfiguration configuration, ILoggerFormatter formatter)
        {
            _formatter = formatter;
            messagesToWriteA = new Queue<string>();
            messagesToWriteB = new Queue<string>();
            UseA = true;

            logFilePath = configuration["Logging:LogFilePath"];

            checkDirecctory();
        }

        public void AddMessage(LoggingMessage message)
        {
            var logEntry = _formatter.Format(message);
            if (UseA)
                messagesToWriteA.Enqueue(logEntry);
            else
                messagesToWriteB.Enqueue(logEntry);
        }

        public async Task ConsumeLogs(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
            UseA = !UseA;
            if (!UseA)
            {
                if (messagesToWriteA.Count > 0)
                {
                    await WriteQueue(messagesToWriteA);
                }
            }
            else
            {
                if (messagesToWriteB.Count > 0)
                {
                    await WriteQueue(messagesToWriteB);
                }
            }
        }

        private async Task WriteQueue(Queue<string> queue)
        {
            checkDirecctory();
            var path = getPath();

            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            fileStream.Seek(0, SeekOrigin.End);
            using (StreamWriter sr = new StreamWriter(fileStream))
            {
                while (queue.Count > 0)
                    await sr.WriteLineAsync(queue.Dequeue()).ConfigureAwait(false);

                sr.Flush();
                sr.Close();
            }
            fileStream.Close();
        }

        private void checkDirecctory()
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new Exception("No entry in appsettings for Logging:LogFilePath");

            if (!logFilePath.EndsWith('/'))
                logFilePath += '/';

            if (!Directory.Exists(logFilePath))
                Directory.CreateDirectory(logFilePath);
        }

        public string getPath()
        {
            return logFilePath + "Log_" + DateTime.Now.ToLocalTime().ToShortDateString().Replace('/', '-') + ".txt";
        }

        public void ServerLogInfo(string message)
        {
            AddMessage(new LoggingMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                Level = "Info",
                RetryCount = 0,
                Timestamp = DateTime.Now,
                Source = "Server",
                ClientIp = "127.0.0.1"
            });
        }

        public void ServerLogWarning(string message)
        {
            AddMessage(new LoggingMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                Level = "Warning",
                RetryCount = 0,
                Timestamp = DateTime.Now,
                Source = "Server",
                ClientIp = "127.0.0.1"
            });
        }

        public void ServerLogError(string message)
        {
            AddMessage(new LoggingMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                Level = "Error",
                RetryCount = 0,
                Timestamp = DateTime.Now,
                Source = "Server",
                ClientIp = "127.0.0.1"
            });
        }

        public void ServerLogInfo(string format, params object[] args)
        {
            ServerLogInfo(string.Format(format, args));
        }

        public void ServerLogWarning(string format, params object[] args)
        {
            ServerLogWarning(string.Format(format, args));
        }

        public void ServerLogError(string format, params object[] args)
        {
            ServerLogError(string.Format(format, args));
        }
    }
}
