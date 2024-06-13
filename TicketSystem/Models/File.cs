using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("files", Schema = "suz")]
    public class File
    {
        public long Id { get; set; }
        public string Path { get; set; }

        public SendFile ToSend()
        {
            return new SendFile() { Id = Id, Path = Path };
        }

        public override string ToString()
        {
            return $"File id:{Id}, path: {Path}";
        }
    }

    public record struct SendFile(long Id, string Path);
}
