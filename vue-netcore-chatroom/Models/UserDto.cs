using System;
namespace vue_netcore_chatroom.Models
{
	public class UserDto
	{
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Location { get; set; }

        public string Bio { get; set; }

        public bool Archived { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public static UserDto FromDbModel(User dbModel)
        {
            var dto = new UserDto()
            {
                Id = dbModel.Id,
                Email = dbModel.Email,
                FirstName = dbModel.FirstName,
                LastName = dbModel.LastName,
                Location = dbModel.Location,
                Bio = dbModel.Bio,
                Archived = dbModel.Archived,
                CreatedAt = dbModel.CreatedAt,
                UpdatedAt = dbModel.UpdatedAt
            };

            return dto;
        }
    }
}

