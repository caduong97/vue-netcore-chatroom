using System;
namespace vue_netcore_chatroom.Models
{
	public class User
	{
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string? HashedPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName)
            ? FirstName + " " + LastName
            : FirstName;

        public string? Location { get; set; }

        public string? Bio { get; set; }

        public bool Archived { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<ChatUser> ChatUsers { get; set; }

    }
}

