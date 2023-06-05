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
            //optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=123qwe!@#QWE;database=suz");
            optionsBuilder.UseMySQL("server=cl-srv-hdsk.cl.local;port=3306;user=xrouu;password=KLyuwi629jasy8HSH;database=suz; Charset=utf8;");
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
