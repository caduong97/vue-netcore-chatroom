using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using vue_netcore_chatroom.Data;
using vue_netcore_chatroom.Helpers;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Services
{
    public interface IUserService
    {
        string EmailFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
        Task<UserDto> CreateSsoUser(ClaimsPrincipal claimsPrincipal);
        Task<User> CreatePasswordUser(RegisterPasswordUserRequest request);
        Task<List<User>> GetAllUsers(ClaimsPrincipal claimsPrincipal);
        Task<UserDto> GetUserByClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
        Task<User> GetUserByEmail(string email);
        Task<UserDto> UpdateUser(UserDto updatedUser);

    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string EmailFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string email;

            try
            {
                email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            }
            catch
            {
                email = "";
            }

            return email;
        }

        private string FirstNameFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string firstName;

            try
            {
                firstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
                if (String.IsNullOrEmpty(firstName))
                {
                    var fullName = claimsPrincipal.FindFirstValue("name");

                    if (String.IsNullOrEmpty(fullName))
                    {
                        firstName = "";
                        return firstName;
                    }

                    var nameParts = fullName.Split(" ");
                    if (nameParts.Length > 0)
                    {
                        firstName = nameParts[0];
                    }
                }
            }
            catch
            {
                firstName = "";
            }

            return firstName;
        }

        private string LastNameFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string lastName;

            try
            {
                lastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname);
                if (String.IsNullOrEmpty(lastName))
                {
                    var fullName = claimsPrincipal.FindFirstValue("name");

                    if (String.IsNullOrEmpty(fullName))
                    {
                        lastName = "";
                        return lastName;
                    }

                    var nameParts = fullName.Split(" ");
                    if (nameParts.Length > 1)
                    {
                        lastName = nameParts[1];
                    }
                }
            }
            catch
            {
                lastName = "";
            }

            return lastName;
        }

        public async Task<UserDto> CreateSsoUser(ClaimsPrincipal claimsPrincipal)
        {
            var email = EmailFromClaimsPrincipal(claimsPrincipal);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser != null)
            {
                return UserDto.FromDbModel(currentUser);
            }

            var newUser = new User();
            newUser.Email = email.Trim().ToLowerInvariant();
            newUser.FirstName = FirstNameFromClaimsPrincipal(claimsPrincipal);
            newUser.LastName = LastNameFromClaimsPrincipal(claimsPrincipal);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();


            return UserDto.FromDbModel(newUser);
        }

        public async Task<User> CreatePasswordUser(RegisterPasswordUserRequest request)
        {
            var user = new User();
            user.Email = request.Email.Trim().ToLowerInvariant();
            user.HashedPassword = PasswordHasher.Hash(request.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            var success = await _context.SaveChangesAsync() > 0;
            if (!success) throw new Exception("Error when creating password user");

            return user;
        }

        public async Task<List<User>> GetAllUsers(ClaimsPrincipal claimsPrincipal)
        {
            var email = EmailFromClaimsPrincipal(claimsPrincipal);

            var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            var users = await _context.Users
                .Where(u => u.Id != me.Id)
                .ToListAsync();

            return users;
        }


        public async Task<UserDto> GetUserByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var email = EmailFromClaimsPrincipal(claimsPrincipal);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new Exception("Cannot find user by claims principal");
            }

            return UserDto.FromDbModel(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var trimmedLowercasedEmail = email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == trimmedLowercasedEmail);

            return user;

        }

        public async Task<UserDto> UpdateUser(UserDto updatedUser)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == updatedUser.Email);

            if (currentUser == null)
            {
                throw new Exception("Error when trying to update user. User with email " + updatedUser.Email + " does not exist.");
            }

            currentUser.FirstName = updatedUser.FirstName;
            currentUser.LastName = updatedUser.LastName;
            currentUser.Location = updatedUser.Location;
            currentUser.Bio = updatedUser.Bio;

            var success = await _context.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new Exception("Error when saving updated user to the database");
            }

            return UserDto.FromDbModel(currentUser);

        }
    }
}
