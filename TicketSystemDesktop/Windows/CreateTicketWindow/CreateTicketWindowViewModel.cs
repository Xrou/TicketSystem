using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using TicketSystemDesktop.Miscellaneous;

namespace TicketSystemDesktop
{
    public class CreateTicketWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        private string text = "";

        private User selectedSender;
        private Company selectedSenderCompany;
        private string selectedSenderPhoneNumber;
        private string selectedSenderEmail;
        private string selectedSenderPCName;

        private bool[] urgency = new bool[] { true, false, false };
        private string selectedFilesText = "";
        private Topic selectedTopic;

        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged("Text"); }
        }
        public User SelectedSender
        {
            get { return selectedSender; }
            set
            {
                selectedSender = value;

                SelectedSenderCompany = AvailableCompanies.First(x => x.Id == value.CompanyId);
                SelectedSenderPhoneNumber = value.PhoneNumber;
                SelectedSenderEmail = value.Email;
                SelectedSenderPCName = value.PCName;

                OnPropertyChanged("SelectedSender");
                OnPropertyChanged("CanEditInfo");
            }
        }
        public Company SelectedSenderCompany
        {
            get { return selectedSenderCompany; }
            set { selectedSenderCompany = value; OnPropertyChanged("SelectedSenderCompany"); }
        }
        public string SelectedSenderPhoneNumber
        {
            get { return selectedSenderPhoneNumber; }
            set { selectedSenderPhoneNumber = value; OnPropertyChanged("SelectedSenderPhoneNumber"); }
        }
        public string SelectedSenderEmail
        {
            get { return selectedSenderEmail; }
            set { selectedSenderEmail = value; OnPropertyChanged("SelectedSenderEmail"); }
        }
        public string SelectedSenderPCName
        {
            get { return selectedSenderPCName; }
            set { selectedSenderPCName = value; OnPropertyChanged("SelectedSenderPCName"); }
        }
        public bool[] Urgency
        {
            get { return urgency; }
            set { urgency = value; OnPropertyChanged("Urgency"); }
        }
        public string SelectedFilesText
        {
            get { return selectedFilesText; }
            set { selectedFilesText = value; OnPropertyChanged("SelectedFilesText"); }
        }
        public Topic SelectedTopic
        {
            get { return selectedTopic; }
            set { selectedTopic = value; OnPropertyChanged("SelectedTopic"); }
        }

        public bool CanEditInfo
        {
            get { return SelectedSender != null; }
        }

        public ObservableCollection<Topic> AvailableTopics { get; set; } = new ObservableCollection<Topic>();
        public ObservableCollection<User> AvailableSenders { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Company> AvailableCompanies { get; set; } = new ObservableCollection<Company>();
        public ObservableCollection<string> SelectedFiles { get; set; } = new ObservableCollection<string>();

        public Window WindowInstance { get; set; }

        public RelayCommand SelectFilesCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = true;

                    if (openFileDialog.ShowDialog() == true)
                    {
                        SelectedFiles.Clear();
                        SelectedFilesText = "";
                        foreach (string filepath in openFileDialog.FileNames)
                        {
                            SelectedFiles.Add(filepath);
                            SelectedFilesText += Path.GetFileName(filepath) + "\n";
                        }
                    }
                });
            }
        }

        public RelayCommand CreateTicketCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        var requestBody = new Dictionary<string, object>();

                        if (SelectedSenderCompany.Id != SelectedSender.CompanyId)
                        {
                            requestBody.Add("companyId", SelectedSenderCompany.Id);
                        }
                        if (SelectedSenderPhoneNumber != SelectedSender.PhoneNumber)
                        {
                            requestBody.Add("phoneNumber", SelectedSenderPhoneNumber);
                        }
                        if (SelectedSenderEmail != SelectedSender.Email)
                        {
                            requestBody.Add("email", SelectedSenderEmail);
                        }
                        if (SelectedSenderPCName != SelectedSender.PCName)
                        {
                            requestBody.Add("pcName", SelectedSenderPCName);
                        }

                        if (requestBody.Count() != 0)
                        {
                            var editResult = HttpClient.Post($"api/users/{SelectedSender.Id}", requestBody);
                        }

                        int urgencyInt = 0;

                        if (Urgency[1] == true)
                        {
                            urgencyInt = 1;
                        }
                        else if (Urgency[2] == true)
                        {
                            urgencyInt = 2;
                        }

                        long senderId = Convert.ToInt64(LocalStorage.Get("MyId")!);

                        if (SelectedSender != null)
                        {
                            senderId = SelectedSender.Id;
                        }

                        requestBody = new Dictionary<string, object> {
                            { "userId", senderId },
                            { "text", Text },
                            { "urgency", urgencyInt.ToString() },
                            { "topicId", selectedTopic.Id },
                        };

                        if (SelectedFiles.Count() != 0)
                        {
                            var filesResult = HttpClient.UploadFile("api/files", SelectedFiles.ToArray());
                            var uploadedFiles = Models.File.ParseArrayFromJson(filesResult.response);
                            requestBody.Add("files", uploadedFiles);
                        }

                        var result = HttpClient.Post("api/tickets", requestBody);

                        if (result.code == System.Net.HttpStatusCode.Created)
                        {
                            Logger.Log(LogStatus.Info, "CreateTicketWindowViewModel.CreateTicketCommand",
                                $"Ticket created with data:\nuserId={senderId}\ntext={Text}\nurgency={urgencyInt.ToString()}\ntopicId={selectedTopic.Id}");

                            Notifications.CallTicketsChanged();
                            WindowInstance.Close();
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "CreateTicketWindowViewModel.CreateTicketCommand",
                                $"Ticket didn't created with data:\nuserId={senderId}\ntext={Text}\nurgency={urgencyInt.ToString()}\ntopicId={selectedTopic.Id}\n\nReturned code:{result.code}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "CreateTicketWindowViewModel.CreateTicketCommand", $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
                    }
                });
            }
        }

        public CreateTicketWindowViewModel(Window window)
        {
            WindowInstance = window;
            Load();
        }

        public void Load()
        {
            try
            {
                var response = HttpClient.Get("api/topics");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Topic.ParseArrayFromJson(response.response);

                    foreach (var t in array)
                    {
                        AvailableTopics.Add(t);
                    }
                }

                if (AvailableTopics.Count > 0)
                    SelectedTopic = AvailableTopics[0];
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "CreateTicketWindowViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }

            try
            {
                var response = HttpClient.Get("api/users");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = User.ParseArrayFromJson(response.response);

                    foreach (var t in array)
                    {
                        AvailableSenders.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "CreateTicketWindowViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }

            try
            {
                var response = HttpClient.Get("api/companies");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Company.ParseArrayFromJson(response.response);

                    foreach (var t in array)
                    {
                        AvailableCompanies.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "CreateTicketWindowViewModel.Load",
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
