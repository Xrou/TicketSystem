using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("comments", Schema = "suz")]
    public class Comment
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public long TicketId { get; set; }
        public long UserId { get; set; }
        [Column("type")]
        public int CommentTypeInt { get; set; }

        [NotMapped]
        public CommentType CommentType
        {
            get
            {
                return (CommentType)CommentTypeInt;
            }
            set
            {
                CommentTypeInt = (int)value;
            }
        }

        virtual public User User { get; set; } = null!;
        virtual public List<File> Files { get; set; } = new List<File>();

        public SendComment ToSend()
        {
            return new SendComment { Id = Id, Text = Text, Date = Date, Type = CommentType, TicketId = TicketId, UserId = User.Id, UserName = User.Name, FullName = User.FullName, Files = Files.Select(x => x.Path).ToArray() };
        }
    }

    public record struct SendComment(long Id, string Text, DateTime Date, CommentType Type, long TicketId, long UserId, string UserName, string FullName, string[] Files);
    public record struct PostComment(long TicketId, string Text, int type);

    public enum CommentType
    {
        Standard = 1,
        Official = 2,
        Service = 3
    }
}
