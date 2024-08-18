using charlie.bll.interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private IChatProvider _chatProvider;

        public ChatController(IChatProvider chatProvider)
        {
            _chatProvider = chatProvider;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetChatRoomList()
        {
            return Ok(await _chatProvider.GetAllMetadataAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetChatRoomChannelHistory([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var history = await _chatProvider.GetChatRoomChannelHistoryAsync(id);
            if (history == null)
                return NotFound();

            return Ok(history);
        }

    }
}
