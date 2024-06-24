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
    public class SettingsWindowCompaniesViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<Company> Companies { get; set; } = new ObservableCollection<Company>();

        public void Load()
        {
            try
            {
                Companies.Clear();

                var response = HttpClient.Get("api/companies");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Company.ParseArrayFromJson(response.response);

                    foreach (var c in array)
                    {
                        Companies.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowCompaniesViewModel.Load",
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
