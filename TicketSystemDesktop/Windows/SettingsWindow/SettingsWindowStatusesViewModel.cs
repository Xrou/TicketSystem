﻿using System;
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
    public class SettingsWindowStatusesViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<Status> Statuses { get; set; } = new ObservableCollection<Status>();

        public void Load()
        {
            try
            {
                Statuses.Clear();

                var response = HttpClient.Get("api/statuses");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Status.ParseArrayFromJson(response.response);

                    foreach (var s in array)
                    {
                        Statuses.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowStatusesViewModel.Load",
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
