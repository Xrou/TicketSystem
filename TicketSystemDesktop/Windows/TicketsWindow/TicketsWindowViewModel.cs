using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace TicketSystemDesktop
{
    public class TicketsWindowViewModel : INotifyPropertyChanged
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
                    window.Show();
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

        public TicketsWindowViewModel()
        {
            var response = HttpClient.Get("api/tickets");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = JsonArray.Parse(response.response);

                foreach (var t in array.AsArray())
                {
                    var ticket = t.AsObject();

                    Tickets.Add(Ticket.ParseFromJson(ticket));                    
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
