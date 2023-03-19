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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace vue_netcore_chatroom.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ChatHub(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }


        static List<ConnectionMapping> ConnectionMappings = new List<ConnectionMapping>();


        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.ReceiveMessage("ChatHub connected");


            if (Context.User != null)
            {
                var user = await _userService.GetUserByClaimsPrincipal(Context.User!);

                var userChatIds = user.ChatUsers.Select(cu => cu.ChatId);

                foreach (var chatId in userChatIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
                }

                var userConnectionMapping = ConnectionMappings
                    .FirstOrDefault(cm => cm.ConnectionId == Context.ConnectionId && cm.Email == user.Email);

                if (userConnectionMapping == null)
                {
                    userConnectionMapping = new ConnectionMapping(Context.ConnectionId, user.Email);
                    ConnectionMappings.Add(userConnectionMapping);
                }


                var callerHubResponse = new HubResponse<List<ConnectionMapping>>(ConnectionMappings);
                await Clients.Caller.AddConnectionMappings(callerHubResponse);

                var allExceptCallerHubResponse = new HubResponse<List<ConnectionMapping>>(
                    new List<ConnectionMapping>() { userConnectionMapping }
                );
                await Clients.AllExcept(Context.ConnectionId).AddConnectionMappings(allExceptCallerHubResponse);

            }
            

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userConnectionMapping = ConnectionMappings.FirstOrDefault(cm => cm.ConnectionId == Context.ConnectionId);

            if (userConnectionMapping != null)
            {
                ConnectionMappings.Remove(userConnectionMapping);

                var allExceptCallerHubResponse = new HubResponse<List<ConnectionMapping>>(
                    new List<ConnectionMapping>() { userConnectionMapping }
                );
                await Clients.AllExcept(Context.ConnectionId).RemoveConnectionMappings(allExceptCallerHubResponse);
            }
            

            await Clients.Caller.ReceiveMessage("ChatHub disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task OnMessageInputFocus(Guid chatId)
        {
            var hubResponse = new HubResponse<MessagingStatus>(
                new MessagingStatus(chatId, true)
            );

            await Clients.OthersInGroup(chatId.ToString()).UpdateMessagingStatus(hubResponse);
        }

        public async Task OnMessageInputBlur(Guid chatId)
        {
            var hubResponse = new HubResponse<MessagingStatus>(
                new MessagingStatus(chatId, false)
            );

            await Clients.OthersInGroup(chatId.ToString()).UpdateMessagingStatus(hubResponse);
        }

    }


    public class MessagingStatus
    {
        public Guid ChatId { get; set; }

        public bool Incoming { get; set; }

        public MessagingStatus(Guid chatId, bool incoming)
        {
            ChatId = chatId;
            Incoming = incoming;
        }
    }

}

