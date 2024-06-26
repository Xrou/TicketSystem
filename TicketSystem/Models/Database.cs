﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace TicketSystem.Models
{
    public class Database : DbContext
    {
        public Database() { }

        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AccessGroup> AccessGroups { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<MergeEntity> Merge { get; set; }  // probably temp entity, used for migration
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Server=cl-srv-suz.cl.local;User ID=suz;password=$UZ5P@$$w0rd;database=suz;Initial Catalog=suz;Trusted_Connection=False;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.ExecutorUser)
                .WithMany()
                .HasForeignKey(e => e.ExecutorId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.ExecutorUser);

            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.SenderUser)
                .WithMany()
                .HasForeignKey(e => e.SenderId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.SenderUser);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.User);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.Subscriptions);

            modelBuilder.Entity<Ticket>()
                .HasMany(e => e.Files)
                .WithMany()
                .UsingEntity("ticketfiles",
                    l => l.HasOne(typeof(File)).WithMany().HasForeignKey("fileId"),
                    r => r.HasOne(typeof(Ticket)).WithMany().HasForeignKey("ticketId"));

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.Files)
                .WithMany()
                .UsingEntity("commentfiles",
                    l => l.HasOne(typeof(File)).WithMany().HasForeignKey("fileId"),
                    r => r.HasOne(typeof(Comment)).WithMany().HasForeignKey("commentId"));

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.Files);

            modelBuilder.Entity<Comment>()
                .Navigation(e => e.Files);

            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.Topic)
                .WithMany()
                .HasForeignKey(e => e.TopicId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.Topic);

            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.Status);

            modelBuilder.Entity<User>()
                .Navigation(e => e.AccessGroup);

            modelBuilder.Entity<User>()
                .Navigation(e => e.Subscriptions);

            modelBuilder.Entity<User>()
                .Navigation(e => e.Company);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserGroups)
                .WithMany(e => e.Users)
                .UsingEntity("useringroup",
                    l => l.HasOne(typeof(UserGroup)).WithMany().HasForeignKey("groupId"),
                    r => r.HasOne(typeof(User)).WithMany().HasForeignKey("userId")
                );

            modelBuilder.Entity<User>()
                .Navigation(e => e.UserGroups);

            modelBuilder.Entity<Comment>()
                .Navigation(e => e.User);

            modelBuilder.Entity<UserGroup>()
                .HasMany(e => e.Users)
                .WithMany(e => e.UserGroups)
                .UsingEntity("useringroup",
                    l => l.HasOne(typeof(User)).WithMany().HasForeignKey("userId"),
                    r => r.HasOne(typeof(UserGroup)).WithMany().HasForeignKey("groupId")
                );

            modelBuilder.Entity<UserGroup>()
                .HasMany(e => e.Companies)
                .WithMany()
                .UsingEntity("companiesingroup",
                    l => l.HasOne(typeof(Company)).WithMany().HasForeignKey("companyId"),
                    r => r.HasOne(typeof(UserGroup)).WithMany().HasForeignKey("groupId")
                );

            modelBuilder.Entity<UserGroup>()
                .HasMany(e => e.Topics)
                .WithMany()
                .UsingEntity("topicsingroup",
                    l => l.HasOne(typeof(Topic)).WithMany().HasForeignKey("topicId"),
                    r => r.HasOne(typeof(UserGroup)).WithMany().HasForeignKey("groupId")
                );

            modelBuilder.Entity<UserGroup>()
                .Navigation(e => e.Companies);

            modelBuilder.Entity<UserGroup>()
                .Navigation(e => e.Topics);
        }
    }
}
