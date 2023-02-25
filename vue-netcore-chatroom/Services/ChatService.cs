using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using vue_netcore_chatroom.Data;
using vue_netcore_chatroom.Hubs;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Services
{
	public interface IChatService
	{
        Task<List<Chat>> GetChats(ClaimsPrincipal claimsPrincipal);
        Task<Chat> CreateOrUpdateChat(ChatDto dto);
        Task<MessageDto> CreateMessage(MessageDto data, ClaimsPrincipal claimsPrincipal);

    }


	public class ChatService : IChatService
	{
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _chatHub;


        public ChatService (ApplicationDbContext context, IUserService userService, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _userService = userService;
            _chatHub = chatHub;
        }

        public async Task<List<Chat>> GetChats(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userService.GetUserByClaimsPrincipal(claimsPrincipal);

            var chatsQuery = _context.Chats
                .Include(c => c.ChatUsers)
                .Include(c => c.Messages);

            List<Chat> chats = chatsQuery
                .AsEnumerable()
                .Where(c => c.ChatUserIds.Any() && c.ChatUserIds.Contains(user.Id))
                .ToList();

            return chats;
        }

        public async Task<Chat> CreateOrUpdateChat(ChatDto dto)
        {
            var updatingChat = dto.Id != Guid.Empty;

            if (updatingChat)
            {
                var existingChat = await _context.Chats
                    .Include(c => c.ChatUsers)
                    .FirstOrDefaultAsync(c => c.Id == dto.Id);
                if (existingChat == null)
                    throw new Exception("Error when updating chat. Chat with id is not found.");

                existingChat.Name = dto.Name;

                var usersToRemove = existingChat.ChatUsers
                    .Where(cu => cu.UserId != null && !dto.ChatUserIds.Contains(cu.UserId.Value))
                    .ToList();
                var usersToAdd = dto.ChatUserIds
                    .Where(cui => !existingChat.ChatUserIds.Contains(cui))
                    .Select(cui => new ChatUser()
                    {
                        ChatId = existingChat.Id,
                        UserId = cui
                    })
                    .ToList();

                foreach(var chatUser in usersToRemove)
                {
                    existingChat.ChatUsers.Remove(chatUser);
                }
                
                existingChat.ChatUsers.AddRange(usersToAdd);
                await _context.SaveChangesAsync();

                return existingChat;
            } else
            {
                Chat newChat = ChatDto.ToDbModel(dto);
                await _context.AddAsync(newChat);

                newChat.ChatUsers = dto.ChatUserIds
                    .Select(cui => new ChatUser()
                    {
                        ChatId = newChat.Id,
                        UserId = cui,
                    })
                    .ToList();

                await _context.SaveChangesAsync();
                return newChat;
            }
        }

        public async Task<MessageDto> CreateMessage(MessageDto data, ClaimsPrincipal claimsPrincipal)
        {
            var chat = await _context.Chats
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.Id == data.SentToChatId);

            if (chat == null)
            {
                throw new Exception("CreateMessage error: Cannot find chat.");
            }

            var chatUser = chat.ChatUsers
                .First(cu => cu.UserId.HasValue && cu.UserId.Value == data.SentByUserId);

            if (chatUser == null)
            {
                throw new Exception("CreateMessage error: Cannot find an existing chat user.");
            }

            Message newMessage = MessageDto.ToDbModel(data);
            newMessage.SentByChatUserId = chatUser.Id;

            _context.Add(newMessage);
            await _context.SaveChangesAsync();

            MessageDto messageDto = MessageDto.FromDbModel(newMessage);

            var chatUserEmails = chat.ChatUsers
               .Where(cu => cu.User != null)
               .Select(cu => cu.User!.Email)
               .ToList();

            var hubResponse = new HubResponse<MessageDto>(messageDto);

            await _chatHub.Clients.Users(chatUserEmails).SendAsync("BroadcastChatMessage", hubResponse);

            return messageDto;
        }

    }
}

