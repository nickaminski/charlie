using charlie.common.exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using charlie.bll.interfaces;

namespace charlie.api.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        ILogWriter _logger;
        public int Order => int.MaxValue - 10;
        public HttpResponseExceptionFilter(ILogWriter logger) {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException httpResponseException)
            {
                _logger.ServerLogError($"Error in action: {context.ActionDescriptor.DisplayName} -- {httpResponseException.Message}");
                context.Result = new ObjectResult(httpResponseException.Value)
                {
                    StatusCode = httpResponseException.StatusCode
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
