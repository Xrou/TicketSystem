using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("usergroups")]
    public class UserGroup
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public List<User> Users { get; set; } = null!;
    }
}
