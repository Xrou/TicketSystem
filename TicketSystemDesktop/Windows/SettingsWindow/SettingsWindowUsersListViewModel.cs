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
    public class SettingsWindowUsersListViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public void Load()
        {
            Users.Clear();
         
            var response = HttpClient.Get("api/users");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = User.ParseArrayFromJson(response.response);

                foreach (var u in array)
                {
                    Users.Add(u);
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
