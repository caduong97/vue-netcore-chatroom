using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace vue_netcore_chatroom.Hubs
{
    [Authorize]
	public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);

    }
}

