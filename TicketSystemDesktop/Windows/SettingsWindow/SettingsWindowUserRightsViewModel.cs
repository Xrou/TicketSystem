using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowUserRightsViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<UserAccessGroup> UserAccessGroups { get; set; } = new ObservableCollection<UserAccessGroup>();
        public List<UserAccessGroup> SourceUserAccessGroups = new List<UserAccessGroup>();

        public RelayCommand SaveUserAccessGroupsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Dictionary<string, object> changes = new Dictionary<string, object>();
                    foreach (var userAccessGroup in UserAccessGroups)
                    {
                        var compareUser = SourceUserAccessGroups.FirstOrDefault(u => u.Id == userAccessGroup.Id);
                        if (userAccessGroup.AccessGroupId != compareUser?.AccessGroupId)
                        {
                            changes.Add(userAccessGroup.Id.ToString(), userAccessGroup.AccessGroupId);
                        }
                    }

                    var result = HttpClient.Post("api/users/UpdateUsersAccessGroups", changes);
                    if(result.code == System.Net.HttpStatusCode.OK)
                    {
                        Load();
                    }
                });
            }
        }

        public void Load()
        {
            UserAccessGroups.Clear();
            SourceUserAccessGroups.Clear();

            var response = HttpClient.Get("api/Users/GetUsersAccessGroup");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = UserAccessGroup.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    UserAccessGroups.Add(t);
                    SourceUserAccessGroups.Add(t.Clone() as UserAccessGroup);
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
