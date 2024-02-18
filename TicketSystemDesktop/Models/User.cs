using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TicketSystemDesktop.Models
{
    public class User : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string fullName;
        private string email;
        private string phoneNumber;
        private long companyId;
        private string companyName;
        private string companyShortName;
        private long telegramId;

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
        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
                OnPropertyChanged("FullName");
            }
        }
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        public long CompanyId
        {
            get
            {
                return companyId;
            }
            set
            {
                companyId = value; 
                OnPropertyChanged("CompanyId");
            }
        }
        public string CompanyName
        {
            get
            {
                return companyName;
            }
            set
            {
                companyName = value; 
                OnPropertyChanged("CompanyName");
            }
        }
        public string CompanyShortName
        {
            get
            {
                return companyShortName;
            }
            set
            {
                companyShortName = value; 
                OnPropertyChanged("CompanyShortName");
            }
        }
        public long TelegramId
        {
            get
            {
                return telegramId;
            }
            set
            {
                telegramId = value; 
                OnPropertyChanged("TelegramId");
            }
        }

        public static User ParseFromJson(JsonObject user)
        {
            var id = user["id"].GetValue<long>();
            var companyId = user["companyId"].GetValue<long>();
            var companyName = user["companyName"].GetValue<string>();
            var companyShortName = user["companyShortName"].GetValue<string>();
            var accessGroupName = user["accessGroupName"].GetValue<string>();
            var fullName = user["fullName"].GetValue<string>();
            var email = user["email"].GetValue<string>();
            var phoneNumber = user["phoneNumber"].GetValue<string>();
            var telegram = user["telegram"].GetValue<long>();

            return new User()
            {
                Id = id,
                FullName = fullName,
                CompanyId = companyId,
                CompanyName = companyName,
                CompanyShortName = companyShortName,
                Email = email,
                PhoneNumber = phoneNumber,
                TelegramId = telegram,
            };
        }

        public static List<User> ParseArrayFromJson(string response)
        {
            List<User> parsed = new List<User>();
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
