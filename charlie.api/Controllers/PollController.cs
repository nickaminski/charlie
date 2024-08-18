using charlie.api.Hubs;
using charlie.bll.interfaces;
using charlie.dto;
using charlie.dto.Poll;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : ControllerBase
    {
        IPollProvider _pollProvider;
        IPollResultsProvider _resultsProvider;
        ITimeProvider _time;
        IHubContext<MessageHub> _messageHub;

        public PollController(IPollProvider pollProvider, 
                              IPollResultsProvider resultsProvider, 
                              IHubContext<MessageHub> messageHub,
                              ITimeProvider time)
        {
            _pollProvider = pollProvider;
            _resultsProvider = resultsProvider;
            _messageHub = messageHub;
            _time = time;
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> CreatePoll([FromBody]PollViewModel newPoll)
        {
            var pollId = await _pollProvider.CreatePoll(newPoll);
            newPoll.id = pollId;
            return Ok(newPoll);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {   var clientIp = GetClientIp();
            var polls = await _pollProvider.GetAll(clientIp);
            return Ok(polls);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPoll([FromQuery]string id)
        {
            var clientIp = GetClientIp();
            var poll = await _pollProvider.GetPoll(id, clientIp);
            if (poll == null)
                return NotFound();

            return Ok(poll);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPollResults([FromQuery]string id)
        {
            var clientIp = GetClientIp();
            var results = await _resultsProvider.GetPollResults(id, clientIp);
            if (results == null)
                return NotFound();

            return Ok(results);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubmitPollResponse([FromBody] SubmitPollResponse response)
        {
            var clientIp = GetClientIp();

            var results = await _resultsProvider.SubmitResults(response.id, response.selectedChoice, clientIp);

            if (results)
            {
                var userName = System.Text.Encoding.UTF8.GetBytes(string.Format("Pollar:{0}", clientIp));
                var packet = new MessagePacket() { 
                    Username = System.Convert.ToBase64String(userName),
                    Message = response.selectedChoice, 
                    Timestamp = _time.CurrentTimeStamp(),
                    UserId = response.id 
                };
                await _messageHub.Clients.Group(string.Format("Pollar-{0}", response.id)).SendAsync("broadcastToChannel", packet);
            }

            return Ok(results);
        }

        private string GetClientIp()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return Request.HttpContext.Connection.RemoteIpAddress.ToString();
            }
        }
    }
}
