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
    public class Topic : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string name;

        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public static Topic ParseFromJson(JsonObject ticket)
        {
            var Id = ticket["id"]!.GetValue<long>();
            var Name = ticket["name"]!.GetValue<string>();

            return new Topic { Id = Id, Name = Name };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
