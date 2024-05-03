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
    public partial class TicketWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        private Ticket ticket;
        private Window window;
        private int selectedCommentTab = 0;
        private string commentText = "";
        private string? selectedFile;

        public Ticket Ticket
        {
            get { return ticket; }
            set { ticket = value; OnPropertyChanged("Ticket"); }
        }
        public string CommentText
        {
            get { return commentText; }
            set { commentText = value; OnPropertyChanged("CommentText"); }
        }
        public int SelectedCommentTab
        {
            get { return selectedCommentTab; }
            set { selectedCommentTab = value; OnPropertyChanged("SelectedCommentTab"); }
        }
        public string? SelectedFile
        {
            get { return selectedFile; }
            set { selectedFile = value; OnPropertyChanged("SelectedFile"); }
        }
        public string SenderPhone // wrapper for catch text changing event
        {
            get { return Ticket.SenderPhone; }
            set { Ticket.SenderPhone = value; OnPropertyChanged("SenderPhone"); ChangePhoneNumber.Execute(value); }
        }

        public ObservableCollection<Comment> StandardComments { get; set; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> OfficialComments { get; set; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> ServiceComments { get; set; } = new ObservableCollection<Comment>();
        public ObservableCollection<string> CommentFiles { get; set; } = new ObservableCollection<string>();

        public TicketWindowViewModel(Ticket ticket, Window window)
        {
            Ticket = ticket;
            DeadlineDateTime = (DateTime)Ticket.DeadlineTime!;
            window.Title = $"Заявка {Ticket.Id}";
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
