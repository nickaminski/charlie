using charlie.bll.interfaces;
using charlie.dto;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace charlie.api.Hubs
{
    public class MessageHub : Hub
    {
        ILogWriter _logger;
        ITimeProvider _time;

        public MessageHub(ILogWriter logger, ITimeProvider time)
        {
            _logger = logger;
            _time = time;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.ServerLogInfo("signalR generic client connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> joinChannel(string channel)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, channel);
                return true;
            } 
            catch (Exception e) { _logger.ServerLogError(e.Message); return false; }
        }

        public async Task<bool> leaveChannel(string channel)
        {
            try
            {
                if (!string.IsNullOrEmpty(channel) && Context.Items[channel].Equals(true.ToString()))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e) { _logger.ServerLogError(e.Message); return false; }
        }

        public async Task sendMessageToChannel(MessagePacket packet)
        {
            try
            {
                packet.Username = GetUsername();
                packet.Timestamp = _time.CurrentTimeStamp();
                packet.UserId = Context.Items["UserId"].ToString();

                var channelId = packet.ChannelId;
                if (!string.IsNullOrEmpty(channelId))
                {
                    await Clients.Group(channelId).SendAsync("broadcastToChannel", packet);
                    _logger.ServerLogInfo(packet.ToString());
                }
            }
            catch (Exception e) { _logger.ServerLogError(e.Message); }
        }

        private string GetUsername()
        {
            return Context.Items["Username"].ToString();
        }

    }
}
