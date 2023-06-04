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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=1474545;database=suz");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .Navigation(e => e.User)
                .AutoInclude();

            modelBuilder.Entity<Comment>()
                .Navigation(e => e.User)
                .AutoInclude();
        }
    }
}
