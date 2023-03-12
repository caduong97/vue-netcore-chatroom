using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_netcore_chatroom.Models
{
	public class ChatUser
	{
		public int Id  { get; set; }

		[ForeignKey("Chat")]
		public Guid ChatId { get; set; }
		public Chat Chat { get; set; }

        // NOTE: Nullable to support case where a user is deleted from App but all the associated chat users and messages should still persist
        // Declare as nullable so that DeleteBehavior.ClientSetNull can be applied by default
        [ForeignKey("User")]
		public Guid? UserId { get; set; }
		public User? User { get; set; }

		public List<Message> Messages { get; set; }

        public List<MessageSeenByChatUser> SeenMessages { get; set; }

    }
}

