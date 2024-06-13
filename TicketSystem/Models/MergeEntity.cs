using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("oldmerge", Schema = "suz")]
    public class MergeEntity
    {
        public long Id { get; set; }
        public long OldId { get; set; }
        public long NewId { get; set; }
        public string Entity { get; set; }
    }
}
