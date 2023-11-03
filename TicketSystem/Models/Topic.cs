using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("topics", Schema = "suz")]
    public class Topic
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public SendTopic ToSend()
        {
            return new() { id = Id, name = Name };
        }
    }

    public record struct SendTopic(long id, string name);
}
