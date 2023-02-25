using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using vue_netcore_chatroom.Data;
using vue_netcore_chatroom.Models;
using vue_netcore_chatroom.Services;

namespace vue_netcore_chatroom.Hubs
{
    [Authorize]
	public class ChatHub : Hub
    {
        private ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("ReceiveMessage", "ChatHub connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.Caller.SendAsync("ReceiveMessage", "ChatHub disconnected");
            return base.OnDisconnectedAsync(exception);
        }

    }
}

