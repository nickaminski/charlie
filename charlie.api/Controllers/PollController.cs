using charlie.api.Hubs;
using charlie.bll.interfaces;
using charlie.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : ControllerBase
    {
        IPollProvider _pollProvider;
        IPollResultsProvider _resultsProvider;
        ILogWriter _logger;
        ITimeProvider _time;
        IHubContext<MessageHub> _messageHub;

        public PollController(ILogWriter logger, 
                              IPollProvider pollProvider, 
                              IPollResultsProvider resultsProvider, 
                              IHubContext<MessageHub> messageHub,
                              ITimeProvider time)
        {
            _pollProvider = pollProvider;
            _resultsProvider = resultsProvider;
            _logger = logger;
            _messageHub = messageHub;
            _time = time;
        }

        [HttpPut("[action]")]
        public async Task<PollViewModel> CreatePoll([FromBody]PollViewModel newPoll)
        {
             _logger.ServerLogInfo("/Poll/CreatePoll");
            var pollId = await _pollProvider.CreatePoll(newPoll);
            newPoll.id = pollId;
            return newPoll;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PollViewModel>> GetAll()
        {
            _logger.ServerLogInfo("/Poll/GetAll");

            var clientIp = GetClientIp();
            return await _pollProvider.GetAll(clientIp);
        }

        [HttpGet("[action]")]
        public async Task<PollViewModel> GetPoll([FromQuery]string id)
        {
            _logger.ServerLogInfo("/Poll/GetPoll/?id=" + id);
            var clientIp = GetClientIp();
            return await _pollProvider.GetPoll(id, clientIp);
        }

        [HttpGet("[action]")]
        public async Task<PollResults> GetPollResults([FromQuery]string id)
        {
            _logger.ServerLogInfo("/Poll/GetPollResults/?id=" + id);
            var clientIp = GetClientIp();
            return await _resultsProvider.GetPollResults(id, clientIp);
        }

        [HttpPost("[action]")]
        public async Task<bool> SubmitPollResponse([FromBody] SubmitPollResponse response)
        {
            _logger.ServerLogInfo("/Poll/SubmitPollResponse/?id=" + response.id + "&selectedChoice="+response.selectedChoice);
            var clientIp = GetClientIp();

            var results = await _resultsProvider.SubmitResults(response.id, response.selectedChoice, clientIp);

            if (results)
            {
                var userName = System.Text.Encoding.UTF8.GetBytes(string.Format("Pollar:{0}", clientIp));
                var packet = new MessagePacket() { 
                    Username = System.Convert.ToBase64String(userName),
                    Message = response.selectedChoice, 
                    Timestamp = _time.CurrentTimeStamp(), UserId = response.id 
                };
                await _messageHub.Clients.Group(string.Format("Pollar-{0}", response.id)).SendAsync("broadcastToChannel", packet);
            }

            return results;
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
