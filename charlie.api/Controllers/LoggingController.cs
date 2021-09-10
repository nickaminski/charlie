using charlie.bll.interfaces;
using charlie.dto;
using Microsoft.AspNetCore.Mvc;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private ILogWriter _writer;

        public LoggingController(ILogWriter writer)
        {
            _writer = writer;
        }

        [HttpPost]
        public ActionResult Post([FromBody]LoggingMessage message)
        {
            message.ClientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            _writer.AddMessage(message);
            return Ok(true);
        }
    }
}