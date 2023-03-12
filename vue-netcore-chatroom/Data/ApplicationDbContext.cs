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
        public DbSet<MessageSeenByChatUser> MessageSeenByChatUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageSeenByChatUser>()
                .HasKey(msbu => new { msbu.ChatUserId, msbu.MessageId });
            modelBuilder.Entity<MessageSeenByChatUser>()
                .HasOne(msbu => msbu.ChatUser)
                .WithMany(cu => cu.SeenMessages)
                .HasForeignKey(msbu => msbu.ChatUserId);
            modelBuilder.Entity<MessageSeenByChatUser>()
                .HasOne(msbu => msbu.Message)
                .WithMany(m => m.SeenByChatUsers)
                .HasForeignKey(msbu => msbu.MessageId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

