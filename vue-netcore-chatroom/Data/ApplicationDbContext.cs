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
        public DbSet<Chat> Chats {get;set;}
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         
        }
    }
}

