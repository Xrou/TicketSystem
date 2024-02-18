using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowTopicViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();

        public void Load()
        {
            Topics.Clear();

            var response = HttpClient.Get("api/topics");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Topic.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    Topics.Add(t);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
