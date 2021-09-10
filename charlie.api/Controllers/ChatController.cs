using charlie.bll.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private ILogWriter _writer;

        public ChatController(ILogWriter writer)
        {
            _writer = writer;
        }

    }
}
