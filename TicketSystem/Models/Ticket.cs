namespace TicketSystem.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public long UserId { get; set; }
        virtual public User User { get; set; } = null!;

        public SendTicket ToSend()
        {
            return new SendTicket() { Id = Id, Text = Text, Date = Date, UserId = UserId, UserName = User.Name };
        }
    }

    public record struct SendTicket(long Id, string Text, DateTime Date, long UserId, string UserName);
    public record struct PostTicket(string Text);
}
