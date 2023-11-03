using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("ticketsubscribers", Schema = "suz")]
    public class Subscription
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TicketId { get; set; }
    }
}
