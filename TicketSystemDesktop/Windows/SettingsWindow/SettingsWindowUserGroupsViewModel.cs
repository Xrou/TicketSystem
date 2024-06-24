using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowUserGroupsViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<UserGroup> UserGroups { get; set; } = new ObservableCollection<UserGroup>();

        public void Load()
        {
            try
            {
                UserGroups.Clear();

                var response = HttpClient.Get("api/userGroups");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = JsonArray.Parse(response.response);

                    foreach (var t in array.AsArray())
                    {
                        var userGroup = t.AsObject();

                        UserGroups.Add(UserGroup.ParseFromJson(userGroup));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowUserGroupsViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;

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
