using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ExecutorId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        virtual public User User { get; set; } = null!;
        virtual public User ExecutorUser { get; set; } = null!;

        public SendTicket ToSend()
        {
            return new SendTicket() { Id = Id, Text = Text, Date = Date, UserId = UserId, UserName = User.Name, ExecutorUserId = ExecutorId, ExecutorUserName = ExecutorUser.Name };
        }
    }

    public record struct SendTicket(long Id, string Text, DateTime Date, long UserId, long ExecutorUserId, string UserName, string ExecutorUserName);
    public record struct PostTicket(string Text);
}
