using Microsoft.EntityFrameworkCore;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=cl-srv-suz.cl.local;port=3306;user=xrou;password=1474545mimosH;database=suz; Charset=utf8;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.ExecutorUser)
                .WithMany()
                .HasForeignKey(e => e.ExecutorId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.ExecutorUser)
                .AutoInclude();

            modelBuilder.Entity<Ticket>()
                .HasOne(e => e.SenderUser)
                .WithMany()
                .HasForeignKey(e => e.SenderId);

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.SenderUser)
                .AutoInclude();

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.User)
                .AutoInclude();

            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.Subscriptions)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(e => e.AccessGroup)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(e => e.Subscriptions)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(e => e.Company)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserGroups)
                .WithMany(e => e.Users)
                .UsingEntity("useringroup",
                    l => l.HasOne(typeof(UserGroup)).WithMany().HasForeignKey("groupId"),
                    r => r.HasOne(typeof(User)).WithMany().HasForeignKey("userId")
                );

            modelBuilder.Entity<User>()
                .Navigation(e => e.UserGroups)
                .AutoInclude();

            modelBuilder.Entity<Comment>()
                .Navigation(e => e.User)
                .AutoInclude();

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
                .Navigation(e => e.Companies)
                .AutoInclude();
        }
    }
}
