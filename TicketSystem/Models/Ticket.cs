using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        [Column("type")]
        public int TypeInt { get; set; }
        public long? ExecutorId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeadlineTime { get; set; }
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

        [NotMapped]
        public TicketType Type
        {
            get
            {
                return (TicketType)TypeInt;
            }
            set
            {
                TypeInt = (int)value;
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

            return new SendTicket() { Id = Id, Text = Text, Date = Date.ToString(), DeadlineTime = DeadlineTime?.ToString(), UserId = UserId, UserName = User.Name, SenderCompany = User.Company.Name, ExecutorUserId = ExecutorId, ExecutorUserName = executorName, Type = (int)Type };
        }
    }

    public record struct SendTicket(long Id, string Text, string Date, string? DeadlineTime, long UserId, string SenderCompany, long? ExecutorUserId, string UserName, string ExecutorUserName, int Type);
    public record struct PostTicket(string Text);

    public enum FinishStatus
    {
        Incident = 0,
        Changes = 1
    }

    public enum TicketType
    {
        Standard = 1,
        Registration = 2
    }
}
