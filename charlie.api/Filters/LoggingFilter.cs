using charlie.bll.interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace charlie.api.Filters
{
    public class LoggingFilter : IActionFilter
    {
        ILogWriter _logger;
        public LoggingFilter(ILogWriter logger) {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.ServerLogInfo($"Executing action: {context.ActionDescriptor.DisplayName} with params: {context.HttpContext.Request.QueryString}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
