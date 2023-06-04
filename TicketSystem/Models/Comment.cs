namespace TicketSystem.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public long TicketId { get; set; }
        
        public long UserId { get; set; }
        virtual public User User { get; set; } = null!;

        public SendComment ToSend()
        {
            return new SendComment { Id = Id, Text = Text, Date = Date, TicketId = TicketId, UserName = User.Name };
        }
    }

    public record struct SendComment(long Id, string Text, DateTime Date, long TicketId, string UserName);
    public record struct PostComment(long TicketId, string Text);
}
