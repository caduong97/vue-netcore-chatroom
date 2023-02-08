using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Data
{
	public class DbInitializer
	{
        public static async Task Initialize(ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            context.Database.Migrate();

            if (env.IsDevelopment())
            {
                await SeedData(context, env, configuration);
            }
        }

        public static async Task SeedData(ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            if (context == null)
            {
                throw new ArgumentNullException("DbContext is null");
            }

            if (context.Users.Any())
            {
                return;
            }

            List<User> seedUsers = new List<User>()
            {
                new User() { Email = "ca.duong97@outlook.com", FirstName = "Ca", LastName = "Duong", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            };
            foreach (var user in seedUsers)
            {
                var dbUsers = await context.Users.ToListAsync();
                var existingUser = dbUsers.Find(u => u.Email == user.Email);
                var userExists = existingUser != null;

                if (!userExists)
                {
                    await context.Users.AddAsync(user);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}

