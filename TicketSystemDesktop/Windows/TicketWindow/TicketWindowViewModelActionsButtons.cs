using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
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

        private bool isRegisterTicket;
        public bool IsRegisterTicket { get { return itsMyTicket & isTicketOpen & isRegisterTicket; } set { isRegisterTicket = value; OnPropertyChanged("isRegisterTicket"); } }

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

        public RelayCommand SendCommentCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (CommentText.Trim() == "")
                        return;

                    var requestBody = new Dictionary<string, object>(new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ticketId", Ticket.Id),
                            new KeyValuePair<string, object>("text", CommentText),
                            new KeyValuePair<string, object>("type", SelectedCommentTab + 1),
                        }
                    );

                    if (CommentFiles.Count() != 0)
                    {
                        var filesResult = HttpClient.UploadFile("api/files", CommentFiles.ToArray());
                        var uploadedFiles = Models.File.ParseArrayFromJson(filesResult.response);
                        requestBody.Add("files", uploadedFiles);
                    }

                    var res = HttpClient.Post("api/comments", requestBody);

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

        public RelayCommand AddFilesToCommentCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = true;

                    if (openFileDialog.ShowDialog() == true)
                    {
                        CommentFiles.Clear();

                        foreach (string filepath in openFileDialog.FileNames)
                        {
                            CommentFiles.Add(filepath);
                        }
                    }
                });
            }
        }

        public RelayCommand RegisterUser
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    VerifyRegistrationWindow window = new VerifyRegistrationWindow(Ticket);
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

        public RelayCommand FileDownloadCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (SelectedFile != null)
                    {
                        string url = $"{Constants.DownloadUrl}/{SelectedFile}";

                        try
                        {
                            Process.Start(url);
                        }
                        catch
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                url = url.Replace("&", "^&");
                                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                            }
                            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                Process.Start("xdg-open", url);
                            }
                            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                Process.Start("open", url);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                });
            }
        }

        public RelayCommand FileDownloadFromCommentCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    string url = $"{Constants.DownloadUrl}/{obj}";

                    try
                    {
                        Process.Start(url);
                    }
                    catch
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            url = url.Replace("&", "^&");
                            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Process.Start("xdg-open", url);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Process.Start("open", url);
                        }
                        else
                        {
                            throw;
                        }
                    }
                });
            }
        }


    }
}
