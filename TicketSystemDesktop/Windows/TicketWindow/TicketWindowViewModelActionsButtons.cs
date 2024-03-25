using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketWindowViewModel
    {
        private bool itsMyTicket;
        public bool ItsMyTicket { get { return itsMyTicket & isTicketOpen; } set { itsMyTicket = value; OnPropertyChanged("ItsMyTicket"); OnPropertyChanged("ItsNotMyTicket"); } }
        public bool ItsNotMyTicket { get { return !itsMyTicket & isTicketOpen; } set { itsMyTicket = !value; OnPropertyChanged("ItsMyTicket"); OnPropertyChanged("ItsNotMyTicket"); } }

        private bool isTicketOpen;
        public bool IsTicketOpen { get { return isTicketOpen; } set { isTicketOpen = value; OnPropertyChanged("IsTicketOpen"); OnPropertyChanged("IsTicketClosed"); } }
        public bool IsTicketClosed { get { return !isTicketOpen; } set { isTicketOpen = !value; OnPropertyChanged("IsTicketOpen"); OnPropertyChanged("IsTicketClosed"); } }

        private bool iSubscribedToTicket = false;
        public bool ISubscribedToTicket { get { return iSubscribedToTicket & isTicketOpen; } set { iSubscribedToTicket = value; OnPropertyChanged("ISubscribedToTicket"); OnPropertyChanged("INotSubscribedToTicket"); } }
        public bool INotSubscribedToTicket { get { return !iSubscribedToTicket & isTicketOpen; } set { iSubscribedToTicket = !value; OnPropertyChanged("ISubscribedToTicket"); OnPropertyChanged("INotSubscribedToTicket"); } }

        private DateTime deadlineDateTime;
        public DateTime DeadlineDateTime { get { return deadlineDateTime; } set { deadlineDateTime = value; OnPropertyChanged("DeadlineDateTime"); } }

        public RelayCommand TakeTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var response = HttpClient.Post("api/tickets/assignTicket", new Dictionary<string, object>()
                    {
                        { "userId", LocalStorage.Get("MyId").ToString() },
                        { "ticketId", Ticket.Id.ToString() }
                    });

                    if (response.code == System.Net.HttpStatusCode.OK)
                    {
                        LoadTicket();
                    }
                });
            }
        }

        public RelayCommand ReopenTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Console.WriteLine(ItsMyTicket);
                    Console.WriteLine(ISubscribedToTicket);
                    Console.WriteLine(IsTicketOpen);
                });
            }
        }

        public RelayCommand AssignTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    ObservableCollection<IDbEntity> users = new ObservableCollection<IDbEntity>();

                    var response = HttpClient.Get("api/users");

                    if (response.code == System.Net.HttpStatusCode.OK)
                    {
                        var array = User.ParseArrayFromJson(response.response);

                        foreach (var c in array)
                        {
                            users.Add(c);
                        }
                    }

                    SelectorWindow selectorWindow = new SelectorWindow(users);
                    selectorWindow.ShowDialog();

                    var selection = (User?)selectorWindow.GetSelectedEntity();

                    if (selection != null)
                    {
                        response = HttpClient.Post("api/tickets/assignTicket", new Dictionary<string, object>()
                        {
                            { "userId", selection.Id.ToString() },
                            { "ticketId", Ticket.Id.ToString() }
                        });

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            LoadTicket();
                        }
                    }
                });
            }
        }

        public RelayCommand SubscribeToTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var response = HttpClient.Get($"api/tickets/subscribe/{Ticket.Id}");
                });
            }
        }

        public RelayCommand CloseTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CloseTicketWindow window = new CloseTicketWindow(Ticket.Id);
                    window.ShowDialog();

                    if (window.GetResult())
                    {
                        Load();
                    }
                });
            }
        }

        public RelayCommand SetTicketDeadline
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var response = HttpClient.Post("api/Tickets/SetDeadline", new Dictionary<string, object>()
                    {
                        { "ticketId", ticket.Id.ToString() },
                        { "deadlineTime", DeadlineDateTime.ToString() }
                    });
                });
            }
        }

    }
}
