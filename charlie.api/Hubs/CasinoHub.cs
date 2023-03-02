using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using charlie.bll.interfaces;
using Microsoft.AspNetCore.SignalR;

namespace charlie.api.Hubs
{
    public class CasinoHub : Hub
    {
        ILogWriter _logger;
        IUserProvider _userProv;
        ITimeProvider _time;

        public CasinoHub(ILogWriter logger, IUserProvider userProv, ITimeProvider time)
        {
            _logger = logger;
            _time = time;
            _userProv = userProv;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.ServerLogInfo("signalR casino client connected");
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
    }
}
