using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("statuses", Schema = "suz")]
    public class Status
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public SendStatus ToSend()
        {
            return new SendStatus() { Id = Id, Name = Name };
        }
    }

    public record struct SendStatus(long Id, string Name);
}
