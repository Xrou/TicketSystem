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
    // Class is not represented in database. It used for correct connectivity between modules in program
    public class Urgency : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string name;

        public long Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public object MainParameter
        {
            get { return name; }
        }

        public static Urgency ParseFromJson(JsonObject ticket)
        {
            var Id = ticket["id"]!.GetValue<long>();
            var Name = ticket["name"]!.GetValue<string>();

            return new Urgency { Id = Id, Name = Name };
        }


        public static List<Urgency> ParseArrayFromJson(string response)
        {
            List<Urgency> parsed = new List<Urgency>();
            var array = JsonArray.Parse(response);

            foreach (var t in array.AsArray())
            {
                var obj = t.AsObject();
                parsed.Add(ParseFromJson(obj));
            }

            return parsed;
        }

        public static List<Urgency> GetUrgencies()
        {
            return new List<Urgency>() { 
                new Urgency() { Id = 0, Name = "Низкая"},
                new Urgency() { Id = 1, Name = "Средняя"},
                new Urgency() { Id = 2, Name = "Высокая"},
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
