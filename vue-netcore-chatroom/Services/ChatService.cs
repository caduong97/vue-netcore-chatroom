using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using vue_netcore_chatroom.Data;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Services
{
	public interface IChatService
	{
        Task<List<Chat>> GetChats(ClaimsPrincipal claimsPrincipal);
        Task<Chat> CreateOrUpdateChat(ChatDto dto);

    }


	public class ChatService : IChatService
	{
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ChatService (ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<List<Chat>> GetChats(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userService.GetUserByClaimsPrincipal(claimsPrincipal);

            var chatsQuery = _context.Chats
                .Include(c => c.ChatUsers);

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
    }
}

