using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketWindowViewModel : INotifyPropertyChanged
    {
        private Ticket ticket;
        private int selectedCommentTab = 0;
        private string commentText = "";
        
        public Ticket Ticket { get { return ticket; } set { ticket = value; OnPropertyChanged("Ticket"); } }
        public string CommentText { get { return commentText; } set { commentText = value; OnPropertyChanged("CommentText"); } }
        public int SelectedCommentTab { get { return selectedCommentTab; } set { selectedCommentTab = value; OnPropertyChanged("SelectedCommentTab"); } }

        public ObservableCollection<Comment> StandardComments { get; set; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> OfficialComments { get; set; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> ServiceComments { get; set; } = new ObservableCollection<Comment>();

        public TicketWindowViewModel(Ticket ticket)
        {
            Ticket = ticket;
            LoadComments();
        }

        public RelayCommand SendCommentCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (CommentText.Trim() == "")
                        return;

                    var res = HttpClient.Post("api/comments",
                        new Dictionary<string, object>(new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ticketId", Ticket.Id),
                            new KeyValuePair<string, object>("text", CommentText),
                            new KeyValuePair<string, object>("type", SelectedCommentTab + 1),
                        }
                      )
                    );

                    if (res.code != System.Net.HttpStatusCode.Created)
                    {
                        MessageBox.Show($"Ошибка в отправлении комментария\n\n{res.code}\n{res.response}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.None);
                    }
                    else
                    {
                        CommentText = "";
                        LoadComments();
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
