using System;
using System.ComponentModel.DataAnnotations;

namespace vue_netcore_chatroom.Models
{
	public class Chat
	{
		public Guid Id { get; set; }

		[MaxLength(255)]
		public string Name { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public List<ChatUser> ChatUsers { get; set; }

        public List<Guid> ChatUserIds => ChatUsers?.Where(cu => cu.UserId.HasValue).Select(cu => cu.UserId.Value).ToList();

		public List<Message> Messages { get; set; }
	}
}

