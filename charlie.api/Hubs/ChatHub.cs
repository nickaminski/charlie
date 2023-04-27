﻿using charlie.bll.interfaces;
using charlie.dto.User;
using charlie.dto;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace charlie.api.Hubs
{
    public class ChatHub : Hub
    {
        ILogWriter _logger;
        IUserProvider _userProv;
        ITimeProvider _time;
        IChatProvider _chatProv;

        public ChatHub(ILogWriter logger, IUserProvider userProv, ITimeProvider time, IChatProvider chatProv)
        {
            _logger = logger;
            _time = time;
            _userProv = userProv;
            _chatProv = chatProv;
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

        public async Task<bool> joinChannel(string channelId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, channelId);

                _logger.ServerLogInfo("client joining channel: {0}", channelId);
                var user = await _userProv.GetUserById(Context.Items["UserId"].ToString());
                user.Channels.Add(channelId);
                var tasks = new List<Task>() {
                    _userProv.SaveUser(new UpdateUser() { Id = user.UserId, Channels = user.Channels }),
                    _chatProv.JoinChatRoom(channelId, user.UserId)
                };
                await Task.WhenAll(tasks);
                _logger.ServerLogInfo("{0} has joined {1}", GetUsername(), channelId);

                return true;
            }
            catch (Exception e) { _logger.ServerLogError(e.Message); return false; }
        }

        public async Task<bool> leaveChannel(string channelId)
        {
            try
            {
                if (!string.IsNullOrEmpty(channelId) && Context.Items[channelId].Equals(true.ToString()))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);

                    var user = await _userProv.GetUserById(Context.Items["UserId"].ToString());
                    user.Channels.Remove(channelId);
                    var tasks = new List<Task>() {
                        _userProv.SaveUser(new UpdateUser() { Id = user.UserId, Channels = user.Channels }),
                        _chatProv.LeaveChatRoom(channelId, user.UserId)
                    };
                    await Task.WhenAll(tasks);
                    
                    var message = string.Format("{0} has left", GetUsername(), channelId);
                    await SystemMessage(message, channelId);
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
                var tasks = new List<Task>();
                if (packet.Username != "System")
                {
                    packet.Username = GetUsername();
                    packet.Timestamp = _time.CurrentTimeStamp();
                    packet.UserId = Context.Items["UserId"].ToString();
                }

                var channelId = packet.ChannelId;
                if (!string.IsNullOrEmpty(channelId))
                {
                    await Task.WhenAll(new List<Task>() 
                    {
                        _chatProv.SaveMessageToChatRoomChannel(packet),
                        Clients.Group(channelId).SendAsync("broadcastToChannel", packet)
                    });
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
