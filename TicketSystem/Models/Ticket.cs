using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long SenderId { get; set; }
        [Column("type")]
        public int TypeInt { get; set; }
        public long? ExecutorId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        [Column("urgency")]
        public int UrgencyInt { get; set; }
        public DateTime? DeadlineTime { get; set; }
        public bool Finished { get; set; }
        [Column("finishStatus")]
        public int FinishStatusInt { get; set; }
        public long TopicId { get; set; }
        public long StatusId { get; set; }

        [NotMapped]
        public Urgency Urgency
        {
            get
            {
                return (Urgency)UrgencyInt;
            }
            set
            {
                UrgencyInt = (int)value;
            }
        }

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
        virtual public User SenderUser { get; set; } = null!;
        virtual public User? ExecutorUser { get; set; }
        virtual public Topic? Topic { get; set; } = null!;
        virtual public Status? Status { get; set; } = null!;

        public SendTicket ToSend()
        {
            string executorName = "";

            if (this.ExecutorUser != null)
                executorName = this.ExecutorUser.FullName;

            string userName = User.FullName;

            if (this.SenderId != this.UserId)
                userName += $" ({SenderUser.FullName})";

            return new SendTicket() { Id = Id, Text = Text, Date = Date.ToString(), DeadlineTime = DeadlineTime?.ToString(), UserId = UserId, SenderId = SenderId, SenderPhone = User.PhoneNumber, UserName = userName, SenderCompany = User.Company.Name, ExecutorUserId = ExecutorId, ExecutorUserName = executorName, Type = (int)Type, Urgency = (int)Urgency, TopicName = Topic.Name, Status = Status.Name };
        }
    }

    public record struct SendTicket(long Id, string Text, string Date, string? DeadlineTime, long UserId, long SenderId, string SenderPhone, string SenderCompany, long? ExecutorUserId, string UserName, string ExecutorUserName, int Type, int Urgency, string TopicName, string Status);
    public record struct PostTicket(long UserId, string Text, int Urgency, long TopicId);

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

    public enum Urgency
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
