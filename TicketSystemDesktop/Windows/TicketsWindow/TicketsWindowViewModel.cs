using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketsWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<Ticket> Tickets { get; set; } = new ObservableCollection<Ticket>();
            
        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var ticket = obj as Ticket;

                    TicketWindow window = new TicketWindow(ticket);
                    window.ShowDialog();
                });
            }
        }

        public RelayCommand OpenCreateTicketWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CreateTicketWindow createTicketWindow = new CreateTicketWindow();
                    createTicketWindow.Show();
                });
            }
        }

        public RelayCommand OpenSettingsWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SettingsWindow settingsWindow = new SettingsWindow();
                    settingsWindow.Show();
                });
            }
        }

        public TicketsWindowViewModel()
        {
            Notifications.TicketsChanged += TicketsChangedOutside;

            Load();
        }

        private void TicketsChangedOutside()
        {
            Load();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
