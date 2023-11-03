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

        public SendComment ToSend()
        {
            return new SendComment { Id = Id, Text = Text, Date = Date, Type = CommentType, TicketId = TicketId, UserName = User.Name };
        }
    }

    public record struct SendComment(long Id, string Text, DateTime Date, CommentType Type, long TicketId, string UserName);
    public record struct PostComment(long TicketId, string Text, int type);

    public enum CommentType
    {
        Standard = 1,
        Official = 2,
        Service = 3
    }
}
