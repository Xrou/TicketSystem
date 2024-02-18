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
    public class Company : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string name;
        private string shortName;

        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public string ShortName { get { return shortName; } set { shortName = value; OnPropertyChanged("ShortName"); } }


        public static Company ParseFromJson(JsonObject companyObject)
        {
            var id = companyObject["id"].GetValue<long>();
            var name = companyObject["name"].GetValue<string>();
            var shortName = companyObject["shortName"].GetValue<string>();

            return new Company() { Id = id, Name = name, ShortName = shortName };
        }

        public static List<Company> ParseArrayFromJson(string response)
        {
            List<Company> parsed = new List<Company>();
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
