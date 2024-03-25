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
    public class Status : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string name;

        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public object MainParameter { get { return name; } }

        public static Status ParseFromJson(JsonObject parseObject)
        {
            var id = parseObject["id"].GetValue<long>();
            var name = parseObject["name"].GetValue<string>();

            return new Status() { Id = id, Name = name };
        }

        public static List<Status> ParseArrayFromJson(string response)
        {
            List<Status> parsed = new List<Status>();
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
