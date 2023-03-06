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
        Task<List<Message>> GetChatMessages(Guid chatId, int startingIndex, int amount);
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
                .Include(c => c.ChatUsers);

            List<Chat> chats = chatsQuery
                .AsEnumerable()
                .Where(c => c.ChatUserIds.Any() && c.ChatUserIds.Contains(user.Id))
                .ToList();

            return chats;
        }

        public async Task<List<Message>> GetChatMessages(Guid chatId, int startingIndex, int amount)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
                throw new Exception("GetChatMessages error. Cannot find chat with id.");

            var messages = await _context.Messages
                .Where(m => !m.ArchivedAt.HasValue && m.SentToChatId == chatId)
                .Include(m => m.SentBy)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var messagesStartingFromIndex = messages.Skip(startingIndex).Take(amount).ToList();

            return messagesStartingFromIndex;
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
            var success = await _context.SaveChangesAsync() > 0;

            if (!success)
            //if (success)
            {
                var failedToSaveMessage = new MessageDto()
                {
                    Id = data.Id,
                    PendingId = data.PendingId,
                    Text = data.Text,
                    SentAt = data.SentAt,
                    SentByUserId = data.SentByUserId,
                    SentToChatId = data.SentToChatId,
                    SavingStatus = MessageSavingStatusEnum.Failed
                };
                var email = _userService.EmailFromClaimsPrincipal(claimsPrincipal);
                var hubResponseToCaller = new HubResponse<MessageDto>(failedToSaveMessage);

                await _chatHub.Clients.User(email).SendAsync("ShowFailedChatMessageToSender", hubResponseToCaller);

                return data;
            }

            MessageDto messageDto = MessageDto.FromDbModel(newMessage);
            messageDto.PendingId = data.PendingId;
            var hubResponse = new HubResponse<MessageDto>(messageDto);

            // TODO: handle exception
            await _chatHub.Clients.Group(chat.Id.ToString()).SendAsync("BroadcastChatMessage", hubResponse);



            return messageDto;
        }

        
    }
}

