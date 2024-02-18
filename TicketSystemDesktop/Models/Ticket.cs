using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop.Models
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
            long? executorUserId = null;
            var id = ticket["id"].GetValue<long>();
            var text = ticket["text"].GetValue<string>();
            var date = DateTime.Parse(ticket["date"].GetValue<string>());
            var deadlineTime = DateTime.Parse(ticket["deadlineTime"].GetValue<string>());

            if (ticket.ContainsKey("executorUserId"))
            {
                executorUserId = ticket["executorUserId"]?.GetValue<long>();
            }

            var executorUserName = ticket["executorUserName"].GetValue<string>();

            var filesList = new List<string>();

            foreach (var file in ticket["files"].AsArray())
            {
                filesList.Add(file.GetValue<string>());
            }

            var files = filesList.ToArray();

            var senderCompany = ticket["senderCompany"].GetValue<string>();
            var senderId = ticket["senderId"].GetValue<long>();
            var senderPhone = ticket["senderPhone"].GetValue<string>();
            var status = ticket["status"].GetValue<string>();
            var topicName = ticket["topicName"].GetValue<string>();
            var type = ticket["type"].GetValue<int>();
            var urgency = ticket["urgency"].GetValue<int>();
            var userId = ticket["userId"].GetValue<long>();
            var userName = ticket["userName"].GetValue<string>();

            return new Ticket()
            {
                Id = id,
                Files = files,
                Date = date,
                DeadlineTime = deadlineTime,
                UserId = userId,
                UserName = userName,
                ExecutorUserId = executorUserId,
                ExecutorUserName = executorUserName,
                SenderCompany = senderCompany,
                SenderId = senderId,
                SenderPhone = senderPhone,
                Status = status,
                TopicName = topicName,
                Text = text,
                TicketType = type,
                Urgency = urgency
            };
        }


        public static List<Ticket> ParseArrayFromJson(string response)
        {
            List<Ticket> parsed = new List<Ticket>();
            var array = JsonArray.Parse(response);

            foreach (var t in array.AsArray())
            {
                var obj = t.AsObject();
                parsed.Add(ParseFromJson(obj));
            }

            return parsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
