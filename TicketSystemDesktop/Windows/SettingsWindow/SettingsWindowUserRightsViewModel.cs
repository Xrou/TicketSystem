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
    public class SettingsWindowUserRightsViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<UserAccessGroup> UserAccessGroups { get; set; } = new ObservableCollection<UserAccessGroup>();

        public void Load()
        {
            UserAccessGroups.Clear();

            var response = HttpClient.Get("api/Users/GetUsersAccessGroup");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = UserAccessGroup.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    UserAccessGroups.Add(t);
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
