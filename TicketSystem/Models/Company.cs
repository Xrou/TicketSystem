using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("companies", Schema = "suz")]
    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public SendCompany ToSend()
        {
            return new SendCompany { id = Id, name = Name, shortName = ShortName };
        }
    }

    public record struct SendCompany(long id, string name, string shortName);
}
