using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? ExecutorId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool Finished { get; set; }
        [Column("finishStatus")]
        public int FinishStatusInt { get; set; }

        [NotMapped]
        public FinishStatus FinishStatus
        {
            get
            {
                return (FinishStatus)FinishStatusInt;
            }
            set
            {
                FinishStatusInt = (int)value;
            }
        }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        virtual public User User { get; set; } = null!;
        virtual public User? ExecutorUser { get; set; }

        public SendTicket ToSend()
        {
            string executorName = "";

            if (this.ExecutorUser != null)
                executorName = this.ExecutorUser.Name;

            return new SendTicket() { Id = Id, Text = Text, Date = Date, UserId = UserId, UserName = User.Name, ExecutorUserId = ExecutorId, ExecutorUserName = executorName };
        }
    }

    public record struct SendTicket(long Id, string Text, DateTime Date, long UserId, long? ExecutorUserId, string UserName, string ExecutorUserName);
    public record struct PostTicket(string Text);

    public enum FinishStatus
    {
        Incident = 0,
        Changes = 1
    }
}
