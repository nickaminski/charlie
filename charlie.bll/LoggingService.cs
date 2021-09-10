using charlie.bll.interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll
{
    public class LoggingService : BackgroundService
    {
        private ILogWriter _logger;

        public LoggingService(ILogWriter logger)
        {
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _logger.ConsumeLogs(stoppingToken);
                }
                catch(OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
