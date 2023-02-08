using System;
using Microsoft.EntityFrameworkCore;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Data
{
	public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}

