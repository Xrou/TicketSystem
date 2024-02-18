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

namespace TicketSystemDesktop
{
    public class CreateTicketWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        private string text = "";
        private bool[] urgency = new bool[] { true, false, false };
        private string selectedFilesText = "";
        private Topic selectedTopic;

        public string Text { get { return text; } set { text = value; OnPropertyChanged("Text"); } }
        public bool[] Urgency { get { return urgency; } set { urgency = value; OnPropertyChanged("Urgency"); } }
        public string SelectedFilesText { get { return selectedFilesText; } set { selectedFilesText = value; OnPropertyChanged("SelectedFilesText"); } }
        public Topic SelectedTopic { get { return selectedTopic; } set { selectedTopic = value; OnPropertyChanged("SelectedTopic"); } }
        public ObservableCollection<Topic> AvailableTopics { get; set; } = new ObservableCollection<Topic>();
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
                    int urgencyInt = 0;

                    if (Urgency[1] == true)
                    {
                        urgencyInt = 1;
                    }
                    else if (Urgency[2] == true)
                    {
                        urgencyInt = 2;
                    }

                    var requestBody = new Dictionary<string, object> {
                        { "userId", LocalStorage.Get("MyId") },
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
                        Notifications.CallTicketsChanged();
                        WindowInstance.Close();
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
