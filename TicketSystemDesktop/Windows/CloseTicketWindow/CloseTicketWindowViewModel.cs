using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TicketSystemDesktop
{
    public class CloseTicketWindowViewModel : INotifyPropertyChanged
    {
        private bool closeTypeIsChanges = true;
        public bool CloseTypeIsChanges { get { return closeTypeIsChanges; } set { closeTypeIsChanges = value; OnPropertyChanged("CloseTypeIsChanges"); } }

        private string closeCommentText = "";
        public string CloseCommentText { get { return closeCommentText; } set { closeCommentText = value; OnPropertyChanged("CloseCommentText"); } }

        private long ticketId;
        private Window window;

        public bool TicketClosed = false;

        public CloseTicketWindowViewModel(Window window, long ticketId)
        {
            this.ticketId = ticketId;
            this.window = window;
        }

        public RelayCommand CloseTicketCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    int finishStatus = CloseTypeIsChanges == true ? 1 : 0;

                    var result = HttpClient.Post("api/tickets/closeTicket",
                        new Dictionary<string, object>() {
                            {"ticketId", ticketId},
                            {"finishStatus", finishStatus},
                            {"commentText", CloseCommentText}
                        }
                    );

                    if (result.code == System.Net.HttpStatusCode.OK)
                    {
                        window.Close();
                        TicketClosed = true;
                    }
                });
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
