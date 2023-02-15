using System;
using System.ComponentModel.DataAnnotations;

namespace vue_netcore_chatroom.Models
{
	public class ChatDto
	{
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Ids of the Users, not the ChatUsers
        public List<Guid> ChatUserIds { get; set; }

        public List<MessageDto> Messages { get; set; }

        public static ChatDto FromDbModel(Chat dbModel)
        {
            var dto = new ChatDto()
            {
                Id = dbModel.Id,
                Name = dbModel.Name,
                CreatedAt = dbModel.CreatedAt,
                UpdatedAt = dbModel.UpdatedAt,
                ChatUserIds = dbModel.ChatUserIds,
                Messages = dbModel.Messages?.Select(m => MessageDto.FromDbModel(m)).ToList() ?? new List<MessageDto>()
            };

            return dto;
        }

        public static Chat ToDbModel(ChatDto dto)
        {
            var dbModel = new Chat()
            {
                Name = dto.Name,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };

            return dbModel;
        }
    }
}

