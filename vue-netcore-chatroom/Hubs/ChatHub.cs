using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using vue_netcore_chatroom.Data;
using vue_netcore_chatroom.Models;
using vue_netcore_chatroom.Services;

namespace vue_netcore_chatroom.Hubs
{
    public class ConnectionMapping
    {
        public string ConnectionId { get; set; }

        public string Email { get; set; }

        public string ChatId { get; set; }

        public ConnectionMapping(string connectionId, string email, string chatId)
        {
            ConnectionId = connectionId;
            Email = email;
            ChatId = chatId;
        }
    }

    [Authorize]
    public class ChatHub : Hub
    {
        private ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ChatHub(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        static List<ConnectionMapping> ChatGroupConnectionMappings = new List<ConnectionMapping>();

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "ChatHub connected");


            if (Context.User != null)
            {
                UserDto user = await _userService.GetUserByClaimsPrincipal(Context.User!);

                var existingConnectionMappings = ChatGroupConnectionMappings
                    .Where(cm => cm.Email == user.Email)
                    .ToList();

                // If there some existing chat group connection mappings here,
                // the user didn't exit the chat room(s) explicitly,
                // because if they did, the connection mappings associated with them would have been removed.
                // Meaning, the user is being reconnected after disconnected here.
                //if (existingConnectionMappings.Any())
                //{                  
                //    foreach (var cm in existingConnectionMappings)
                //    {
                //        // 1st, add user to group again after reconnected
                //        await Groups.AddToGroupAsync(Context.ConnectionId, cm.ChatId);

                //        // 2nd, update the existing connection mapping with new connection id
                //        // Then, send updated connection mapping to clients of the same chat group
                //        cm.ConnectionId = Context.ConnectionId;
                //        var hubResponse = new HubResponse<ConnectionMapping>(cm);
                //        await Clients.Group(cm.ChatId).SendAsync("UserJoinedChat", hubResponse);
                //    }
                //}
            }
            

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.Caller.SendAsync("ReceiveMessage", "ChatHub disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);


        public async Task JoinChat(string chatId)
        {
            Debug.WriteLine("JoinChat");

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            if (Context.User == null)
            {
                throw new Exception("ChatHub JoinChat method error. Cannot get ClaimsPrincipal from Context");
            }

            UserDto user = await _userService.GetUserByClaimsPrincipal(Context.User!);
            ConnectionMapping newConnectionMapping = new ConnectionMapping(Context.ConnectionId, user.Email, chatId);
            ChatGroupConnectionMappings.Add(newConnectionMapping);

            var hubResponse = new HubResponse<List<ConnectionMapping>>(
                ChatGroupConnectionMappings.Where(cm => cm.ChatId == chatId).ToList()
            );
            await Clients.Group(chatId).SendAsync("AddChatGroupConnectionMappings", hubResponse);

        }

        public async Task LeaveChat(string chatId)
        {
            Debug.WriteLine("LeaveChat");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

            if (Context.User == null)
            {
                throw new Exception("ChatHub LeaveChat method error. Cannot get ClaimsPrincipal from Context");
            }

            UserDto user = await _userService.GetUserByClaimsPrincipal(Context.User!);

            // Send hub response to caller to remove all connection mappings of the chat group they are exiting from
            var hubResponseToCaller = new HubResponse<List<ConnectionMapping>>(
                ChatGroupConnectionMappings.Where(cm => cm.ChatId == chatId).ToList()
            );
            await Clients.Caller.SendAsync("RemoveChatGroupConnectionMappings", hubResponseToCaller);

            // Remove the connect mapping associate with this connection and chat id
            var removedConnectionMapping = ChatGroupConnectionMappings
                .First(cm => cm.ConnectionId == Context.ConnectionId && cm.ChatId == chatId);
            if (removedConnectionMapping == null)
            {
                throw new Exception("ChatHub LeaveChat method error. Cannot find existing connection mapping.");
            }
            ChatGroupConnectionMappings.Remove(removedConnectionMapping);

            // Send hub response to group about the connection mappings to remove
            var hubResponseToGroup = new HubResponse<List<ConnectionMapping>>(
                new List<ConnectionMapping>() { removedConnectionMapping }
            );
            await Clients.Group(chatId).SendAsync("RemoveChatGroupConnectionMappings", hubResponseToGroup);
            
        }

        private async Task<ChatUser> getChannelUser(HubCallerContext hubCallerContext, Guid chatId)
        {
            var user = await _userService.GetUserByClaimsPrincipal(hubCallerContext.User);

            var chatUser = await _context.ChatUsers
                .FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId.HasValue && cu.UserId == user.Id);

            return chatUser;
        }
    }
}

