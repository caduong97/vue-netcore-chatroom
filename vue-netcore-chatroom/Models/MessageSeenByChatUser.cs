using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_netcore_chatroom.Models
{
	public class MessageSeenByChatUser
	{
		[ForeignKey("ChatUser")]
		public int ChatUserId { get; set; }
		public ChatUser ChatUser { get; set; }

		[ForeignKey("Message")]
		public int MessageId { get; set; }
		public Message Message { get; set; }
	}
}

