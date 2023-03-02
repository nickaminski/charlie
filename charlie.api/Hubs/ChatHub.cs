using charlie.bll.interfaces;
using charlie.dto.User;
using charlie.dto;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace charlie.api.Hubs
{
    public class ChatHub : Hub
    {
        ILogWriter _logger;
        IUserProvider _userProv;
        ITimeProvider _time;

        public ChatHub(ILogWriter logger, IUserProvider userProv, ITimeProvider time)
        {
            _logger = logger;
            _time = time;
            _userProv = userProv;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.ServerLogInfo("signalR chat client connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> setUpChannels(string id)
        {
            _logger.ServerLogInfo("setting up client channels");
            var user = await _userProv.GetUserById(id);

            if (user == null)
            {
                return false;
            }

            Context.Items["UserId"] = id;
            Context.Items["Username"] = user.Username;
            await Task.WhenAll(user.Channels.Select(x => Groups.AddToGroupAsync(Context.ConnectionId, x)));

            return true;
        }

        public async Task<bool> joinChannel(string channel)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, channel);

                _logger.ServerLogInfo("client joining channel: {0}", channel);
                var user = await _userProv.GetUserById(Context.Items["UserId"].ToString());
                user.Channels.Add(channel);
                await _userProv.SaveUser(new UpdateUser() { Id = user.UserId, Channels = user.Channels });
                _logger.ServerLogInfo("{0} has joined {1}", GetUsername(), channel);

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

                    var user = await _userProv.GetUserById(Context.Items["UserId"].ToString());
                    user.Channels.Remove(channel);
                    await _userProv.SaveUser(new UpdateUser() { Id = user.UserId, Channels = user.Channels });
                    var message = string.Format("{0} has left {1}", GetUsername(), channel);
                    await SystemMessage(message, channel);
                    _logger.ServerLogInfo(message);

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
                if (packet.Username != "System")
                {
                    packet.Username = GetUsername();
                    packet.Timestamp = _time.CurrentTimeStamp();
                    packet.UserId = Context.Items["UserId"].ToString();
                }

                var channelId = packet.ChannelId;
                if (!string.IsNullOrEmpty(channelId))
                {
                    await Clients.Group(channelId).SendAsync("broadcastToChannel", packet);
                    _logger.ServerLogInfo(packet.ToString());
                }
            }
            catch (Exception e) { _logger.ServerLogError(e.Message); }
        }

        private async Task SystemMessage(string message, string channel)
        {
            MessagePacket packet = new MessagePacket
            {
                Id = new Guid().ToString(),
                Message = message,
                Timestamp = _time.CurrentTimeStamp(),
                Username = "System",
                ChannelId = channel
            };
            await sendMessageToChannel(packet);
        }

        private string GetUsername()
        {
            return Context.Items["Username"].ToString();
        }
    }
}
