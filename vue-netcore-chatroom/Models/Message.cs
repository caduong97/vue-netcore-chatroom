using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_netcore_chatroom.Models
{
	public class Message
	{
		public int Id { get; set; }

		public string Text { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public bool Archived => ArchivedAt.HasValue;

        // NOTE: Nullable to support case where a chat user is deleted (removed from chat) but all the messages sent by that user still persist
        // Declare as nullable so that DeleteBehavior.ClientSetNull can be applied by default
        [ForeignKey("SentBy")]
		public int? SentByChatUserId { get; set; }
		public ChatUser? SentBy { get; set; }

        public Guid? SentByUserId => SentBy?.UserId ?? null;

        public string SendByUserName => SentBy?.User?.FullName ?? "";

        [ForeignKey("SentTo")]
        public Guid SentToChatId { get; set; }
        public Chat SentTo { get; set; }

	}
}

