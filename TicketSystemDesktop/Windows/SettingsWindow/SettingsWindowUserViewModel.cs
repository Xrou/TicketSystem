using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowUserViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public User User { get; set; }

        public void Load()
        {
            try
            {
                var response = HttpClient.Get($"api/users/{LocalStorage.Get("MyId")}");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var userObject = JsonNode.Parse(response.response).AsObject();

                    User = User.ParseFromJson(userObject);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowUserViewModel.Load",
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
