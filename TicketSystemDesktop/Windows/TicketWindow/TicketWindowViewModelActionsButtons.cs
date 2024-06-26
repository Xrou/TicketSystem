﻿using Microsoft.Win32;
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
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketWindowViewModel
    {
        private bool itsMyTicket;
        public bool ItsMyTicket
        {
            get { return itsMyTicket & isTicketOpen; }
            set { itsMyTicket = value; OnPropertyChanged("ItsMyTicket"); OnPropertyChanged("ItsNotMyTicket"); }
        }
        public bool ItsNotMyTicket
        {
            get { return !itsMyTicket & isTicketOpen; }
            set { itsMyTicket = !value; OnPropertyChanged("ItsMyTicket"); OnPropertyChanged("ItsNotMyTicket"); }
        }

        private bool isTicketOpen;
        public bool IsTicketOpen
        {
            get { return isTicketOpen; }
            set { isTicketOpen = value; OnPropertyChanged("IsTicketOpen"); OnPropertyChanged("IsTicketClosed"); }
        }
        public bool IsTicketClosed
        {
            get { return !isTicketOpen; }
            set { isTicketOpen = !value; OnPropertyChanged("IsTicketOpen"); OnPropertyChanged("IsTicketClosed"); }
        }

        private bool iSubscribedToTicket = false;
        public bool ISubscribedToTicket
        {
            get { return iSubscribedToTicket & isTicketOpen; }
            set { iSubscribedToTicket = value; OnPropertyChanged("ISubscribedToTicket"); OnPropertyChanged("INotSubscribedToTicket"); }
        }
        public bool INotSubscribedToTicket
        {
            get { return !iSubscribedToTicket & isTicketOpen; }
            set { iSubscribedToTicket = !value; OnPropertyChanged("ISubscribedToTicket"); OnPropertyChanged("INotSubscribedToTicket"); }
        }

        private bool isRegisterTicket;
        public bool IsRegisterTicket
        {
            get { return itsMyTicket & isTicketOpen & isRegisterTicket; }
            set { isRegisterTicket = value; OnPropertyChanged("isRegisterTicket"); }
        }

        private DateTime deadlineDateTime;
        public DateTime DeadlineDateTime
        {
            get { return deadlineDateTime; }
            set { deadlineDateTime = value; OnPropertyChanged("DeadlineDateTime"); }
        }

        public RelayCommand TakeTicket
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        var response = HttpClient.Post("api/tickets/assignTicket", new Dictionary<string, object>()
                        {
                            { "userId", LocalStorage.Get("MyId").ToString() },
                            { "ticketId", Ticket.Id.ToString() }
                        });

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            LoadTicket();
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.TakeTicket",
                                $"Successfully take ticket");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.TakeTicket",
                                $"Cant take ticket with data:\nuserId={LocalStorage.Get("MyId")}\nticketId={Ticket.Id}\n\nReturn code: {response.code}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.TakeTicket",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
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
                    try
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
                                Logger.Log(LogStatus.Info, "TicketsWindowViewModel.AssignTicket",
                                    $"Successfully assigned tiket");
                            }
                            else
                            {
                                Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.AssignTicket",
                                    $"Cant assign ticket with data:\nuserId={selection.Id}\nticketId={Ticket.Id}\n\nReturn code: {response.code}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.AssignTicket",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
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
                    try
                    {
                        var response = HttpClient.Get($"api/tickets/subscribe/{Ticket.Id}");
                        if (response.code == System.Net.HttpStatusCode.Created)
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SubscribeToTicket",
                                $"Successfully subscribed to ticket");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SubscribeToTicket",
                                $"Cant subscribe to ticket with id={Ticket.Id}\n\nReturn code: {response.code}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SubscribeToTicket",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
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
                    try
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
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SendCommentCommand",
                                $"Cant send comment with parameters:\nticketId={Ticket.Id}\ntext={CommentText}\ntype={SelectedCommentTab + 1}\n\nReturn code: {res.code}");
                        }
                        else
                        {
                            CommentText = "";
                            LoadComments();
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SendCommentCommand",
                                $"Successfully sent comment");

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SendCommentCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
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
                    try
                    {
                        var response = HttpClient.Post("api/Tickets/SetDeadline", new Dictionary<string, object>()
                        {
                            { "ticketId", ticket.Id.ToString() },
                            { "deadlineTime", DeadlineDateTime.ToString() }
                        });

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SetTicketDeadline",
                                $"Successfully changed deadline");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SetTicketDeadline",
                                $"Cant change deadline with data:\nticketId={ticket.Id}\ndeadlineTime={DeadlineDateTime}\n\nReturn code: {response.code}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SetTicketDeadline",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
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

        public RelayCommand SelectStatusCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> statuses = new ObservableCollection<IDbEntity>();

                        var response = HttpClient.Get("api/statuses");

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            var array = Status.ParseArrayFromJson(response.response);

                            foreach (var s in array)
                            {
                                statuses.Add(s);
                            }
                        }

                        SelectorWindow selectorWindow = new SelectorWindow(statuses);
                        selectorWindow.ShowDialog();

                        var selectedStatus = (Status?)selectorWindow.GetSelectedEntity();

                        if (selectedStatus == null)
                            return;

                        Ticket.Status = selectedStatus.Name;

                        response = HttpClient.Post("api/tickets/setStatus", new Dictionary<string, object>() {
                            { "ticketId", Ticket.Id.ToString() },
                            { "statusId", selectedStatus.Id.ToString() }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SelectStatusCommand",
                                $"Cant set status with data:\nticketId={Ticket.Id}\nstatusId={selectedStatus.Id}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectStatusCommand",
                                $"Successfully set status");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectStatusCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand SelectSenderCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> users = new ObservableCollection<IDbEntity>();

                        var response = HttpClient.Get("api/users");

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            var array = User.ParseArrayFromJson(response.response);

                            foreach (var s in array)
                            {
                                users.Add(s);
                            }
                        }

                        SelectorWindow selectorWindow = new SelectorWindow(users);
                        selectorWindow.ShowDialog();

                        var selectedSender = (User?)selectorWindow.GetSelectedEntity();

                        if (selectedSender == null)
                            return;

                        response = HttpClient.Post("api/tickets/changeSender", new Dictionary<string, object>() {
                            { "ticketId", Ticket.Id.ToString() },
                            { "userId", selectedSender.Id.ToString() }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SelectSenderCommand",
                                $"Cant change sender with data:\nticketId={ticket.Id}\nuserId={selectedSender.Id}\n\nReturn code: {response.code}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectSenderCommand",
                                 $"Successfully changed sender");
                        }

                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectSenderCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand SelectCompanyCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> companies = new ObservableCollection<IDbEntity>();

                        var response = HttpClient.Get("api/companies");

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            var array = Company.ParseArrayFromJson(response.response);

                            foreach (var s in array)
                            {
                                companies.Add(s);
                            }
                        }

                        SelectorWindow selectorWindow = new SelectorWindow(companies);
                        selectorWindow.ShowDialog();

                        var selectedCompany = (Company?)selectorWindow.GetSelectedEntity();

                        if (selectedCompany == null)
                            return;

                        response = HttpClient.Post($"api/users/{Ticket.UserId}", new Dictionary<string, object>() {
                            { "companyId", selectedCompany.Id }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SelectCompanyCommand",
                                $"Cant change company with data:\ncompanyId={selectedCompany.Id}\nuserId={Ticket.UserId}\n\nReturn code: {response.code}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectCompanyCommand",
                                 $"Successfully changed company");
                        }

                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectCompanyCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand SelectUrgencyCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> urgencies = new ObservableCollection<IDbEntity>(Urgency.GetUrgencies());

                        SelectorWindow selectorWindow = new SelectorWindow(urgencies);
                        selectorWindow.ShowDialog();

                        var selectedUrgency = (Urgency?)selectorWindow.GetSelectedEntity();

                        if (selectedUrgency == null)
                            return;

                        var response = HttpClient.Post($"api/tickets/setUrgency", new Dictionary<string, object>() {
                            { "urgencyId", selectedUrgency.Id.ToString() },
                            { "ticketId", Ticket.Id }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectUrgencyCommand",
                                     $"Cant change urgency with data:\nurgencyId={selectedUrgency.Id}\nticketId={Ticket.Id}\n\nReturn code: {response.code}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectUrgencyCommand",
                                 $"Successfully changed urgency");

                        }

                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectUrgencyCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand SelectTopicCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> topics = new ObservableCollection<IDbEntity>();

                        var response = HttpClient.Get("api/topics");

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            var array = Topic.ParseArrayFromJson(response.response);

                            foreach (var s in array)
                            {
                                topics.Add(s);
                            }
                        }

                        SelectorWindow selectorWindow = new SelectorWindow(topics);
                        selectorWindow.ShowDialog();

                        var selectedTopic = (Topic?)selectorWindow.GetSelectedEntity();

                        if (selectedTopic == null)
                            return;

                        response = HttpClient.Post($"api/tickets/changeTopic", new Dictionary<string, object>() {
                            { "ticketId", Ticket.Id },
                            { "topicId", selectedTopic.Id }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SelectTopicCommand",
                                $"Cant change topic with data:\ncompanyId={Ticket.Id}\ntopicId={selectedTopic.Id}\n\nReturn code: {response.code}");

                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectTopicCommand",
                                 $"Successfully changed topic");
                        }

                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectTopicCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand SelectExecutorCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        ObservableCollection<IDbEntity> users = new ObservableCollection<IDbEntity>();

                        var response = HttpClient.Get("api/users");

                        if (response.code == System.Net.HttpStatusCode.OK)
                        {
                            var array = User.ParseArrayFromJson(response.response);

                            foreach (var s in array)
                            {
                                users.Add(s);
                            }
                        }

                        SelectorWindow selectorWindow = new SelectorWindow(users);
                        selectorWindow.ShowDialog();

                        var selectedExecutor = (User?)selectorWindow.GetSelectedEntity();

                        if (selectedExecutor == null)
                            return;

                        response = HttpClient.Post("api/tickets/assignTicket", new Dictionary<string, object>() {
                            { "ticketId", Ticket.Id.ToString() },
                            { "userId", selectedExecutor.Id.ToString() }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.SelectCompanyCommand",
                                $"Cant change executor with data:\nticketId={Ticket.Id}\nuserId={selectedExecutor.Id}\n\nReturn code: {response.code}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.SelectExecutorCommand",
                                 $"Successfully changed executor");
                        }

                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.SelectExecutorCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

        public RelayCommand ChangePhoneNumber
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        var response = HttpClient.Post($"api/users/{Ticket.UserId}", new Dictionary<string, object>() {
                            { "phoneNumber", obj }
                        });

                        if (response.code != System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show(response.response, response.code.ToString(), MessageBoxButton.OK);
                            Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.ChangePhoneNumber",
                                $"Cant change phone number with data:\nphoneNumber={obj}\nuserId={Ticket.UserId}\n\nReturn code: {response.code}");
                        }
                        else
                        {
                            Logger.Log(LogStatus.Info, "TicketsWindowViewModel.ChangePhoneNumber",
                                 $"Successfully changed phone number");
                        }
                        LoadTicket();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "TicketsWindowViewModel.ChangePhoneNumber",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                    }
                });
            }
        }

    }
}
