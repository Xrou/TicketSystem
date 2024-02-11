using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class Ticket : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string text;
        private DateTime date;
        private DateTime? deadlineTime;
        private long userId;
        private long senderId;
        private string senderPhone;
        private string senderCompany;
        private long? executorUserId;
        private string userName;
        private string executorUserName;
        private int ticketType;
        private int urgency;
        private string topicName;
        private string status;
        private string[] files;

        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value; OnPropertyChanged("Id");
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value; OnPropertyChanged("Text");
            }
        }
        public DateTime Date
        {
            get
            {
                return date;
            }
            set { date = value; OnPropertyChanged("Date"); }
        }
        public DateTime? DeadlineTime
        {
            get
            {
                return deadlineTime;
            }
            set { deadlineTime = value; OnPropertyChanged("DeadlineTime"); }
        }
        public long UserId
        {
            get
            {
                return userId;
            }
            set { userId = value; OnPropertyChanged("UserId"); }
        }
        public long SenderId
        {
            get
            {
                return senderId;
            }
            set { senderId = value; OnPropertyChanged("SenderId"); }
        }
        public string SenderPhone
        {
            get
            {
                return senderPhone;
            }
            set { senderPhone = value; OnPropertyChanged("SenderPhone"); }
        }
        public string SenderCompany
        {
            get
            {
                return senderCompany;
            }
            set { senderCompany = value; OnPropertyChanged("SenderCompany"); }
        }
        public long? ExecutorUserId
        {
            get
            {
                return executorUserId;
            }
            set { executorUserId = value; OnPropertyChanged("ExecutorUserId"); }
        }
        public string UserName
        {
            get
            {
                return userName;
            }
            set { userName = value; OnPropertyChanged("UserName"); }
        }
        public string ExecutorUserName
        {
            get
            {
                return executorUserName;
            }
            set { executorUserName = value; OnPropertyChanged("ExecutorUserName"); }
        }
        public int TicketType
        {
            get
            {
                return ticketType;
            }
            set { ticketType = value; OnPropertyChanged("TicketType"); }
        }
        public int Urgency
        {
            get
            {
                return urgency;
            }
            set { urgency = value; OnPropertyChanged("Urgency"); }
        }
        public string TopicName
        {
            get
            {
                return topicName;
            }
            set { topicName = value; OnPropertyChanged("TopicName"); }
        }
        public string Status
        {
            get
            {
                return status;
            }
            set { status = value; OnPropertyChanged("Status"); }
        }
        public string[] Files
        {
            get
            {
                return files;
            }
            set { files = value; OnPropertyChanged("Files"); }
        }

        public static Ticket ParseFromJson(JsonObject ticket)
        {
            long? ExecutorUserId = null;
            var Id = ticket["id"].GetValue<long>();
            var Text = ticket["text"].GetValue<string>();
            var Date = DateTime.Parse(ticket["date"].GetValue<string>());
            var DeadlineTime = DateTime.Parse(ticket["deadlineTime"].GetValue<string>());

            if (ticket.ContainsKey("executorUserId"))
            {
                ExecutorUserId = ticket["executorUserId"]?.GetValue<long>();
            }

            var ExecutorUserName = ticket["executorUserName"].GetValue<string>();

            var FilesList = new List<string>();

            foreach (var file in ticket["files"].AsArray())
            {
                FilesList.Add(file.GetValue<string>());
            }

            var Files = FilesList.ToArray();

            var SenderCompany = ticket["senderCompany"].GetValue<string>();
            var SenderId = ticket["senderId"].GetValue<long>();
            var SenderPhone = ticket["senderPhone"].GetValue<string>();
            var Status = ticket["status"].GetValue<string>();
            var TopicName = ticket["topicName"].GetValue<string>();
            var @Type = ticket["type"].GetValue<int>();
            var Urgency = ticket["urgency"].GetValue<int>();
            var UserId = ticket["userId"].GetValue<long>();
            var UserName = ticket["userName"].GetValue<string>();

            return new Ticket()
            {
                Id = Id,
                Files = Files,
                Date = Date,
                DeadlineTime = DeadlineTime,
                UserId = UserId,
                UserName = UserName,
                ExecutorUserId = ExecutorUserId,
                ExecutorUserName = ExecutorUserName,
                SenderCompany = SenderCompany,
                SenderId = SenderId,
                SenderPhone = SenderPhone,
                Status = Status,
                TopicName = TopicName,
                Text = Text,
                TicketType = @Type,
                Urgency = Urgency
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
