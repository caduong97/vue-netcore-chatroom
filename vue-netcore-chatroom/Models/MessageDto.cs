using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_netcore_chatroom.Models
{
	public class MessageDto
	{
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime? ArchivedAt { get; set; }

        // Id of the User, not the ChatUser
        public Guid? SentByUserId { get; set; }

        public Guid SentToChatId { get; set; }

        public Guid? PendingId { get; set; }

        public MessageSavingStatusEnum SavingStatus { get; set; }

        public static MessageDto FromDbModel(Message dbModel)
        {
            var dto = new MessageDto()
            {
                Id = dbModel.Id,
                Text = dbModel.Text,
                SentAt = dbModel.SentAt,
                ArchivedAt = dbModel.ArchivedAt,
                SentByUserId = dbModel.SentByUserId,
                SentToChatId = dbModel.SentToChatId
            };

            return dto;
        }

        public static Message ToDbModel(MessageDto dto)
        {
            var dbModel = new Message()
            {
                Id = dto.Id,
                Text = dto.Text,
                SentAt = DateTime.UtcNow,
                ArchivedAt = null,
                SentToChatId = dto.SentToChatId
            };

            return dbModel;
        }
    }

    public enum MessageSavingStatusEnum
    {
        Success = 0,
        Pending,
        Failed
    }
}

