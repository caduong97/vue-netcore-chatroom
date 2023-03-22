using System;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
        Task AddConnectionMappings(HubResponse<List<ConnectionMapping>> hubResponse);
        Task RemoveConnectionMappings(HubResponse<List<ConnectionMapping>> hubResponse);
        Task UpdateChatUsers(HubResponse<ChatDto> hubResponse);
        Task BroadcastChatMessage(HubResponse<MessageDto> hubResponse);
        Task ShowFailedChatMessageToSender(HubResponse<MessageDto> hubResponse);
        Task UpdateMessagingStatus(HubResponse<MessagingStatus> hubResponse);
        Task UpdateMessagesSeenByUsers(HubResponse<List<MessageDto>> hubResponse);
    }
}

