using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Reflection.Emit;
using System.Windows.Controls;
using System.Threading;

namespace TicketSystemDesktop
{
    public class LoginWindowViewModel : INotifyPropertyChanged
    {
        public Window WindowInstance { get; set; }

        public LoginWindowViewModel(Window window)
        {
            this.WindowInstance = window;
            // only debug
            /*
            var result = HttpClient.Post("api/users/login",
                        new Dictionary<string, object>() {
                            { "login", "SuperAdmin" },
                            { "password", "123456789" }
                        }, false);

            if (result.code == System.Net.HttpStatusCode.OK)
            {
                var responseJson = JsonNode.Parse(result.response!);

                LocalStorage.Set("AccessToken", responseJson["access_token"]!.GetValue<string>());
                LocalStorage.Set("MyUserName", responseJson["username"]!.GetValue<string>());
                LocalStorage.Set("MyId", responseJson["id"]!.GetValue<long>().ToString());

                Window newWindow = new TicketsWindow();
                newWindow.Show();

                WindowInstance.Close();
            }*/
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username"); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }

        private Visibility incorrectAuthVisible = Visibility.Hidden;
        public Visibility IncorrectAuthVisible
        {
            get { return incorrectAuthVisible; }
            set { incorrectAuthVisible = value; OnPropertyChanged("IncorrectAuthVisible"); }
        }

        public RelayCommand AuthorizeCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var result = HttpClient.Post("api/users/login",
                        new Dictionary<string, object>() {
                            { "login", Username },
                            { "password", Password }
                        }, false);

                    if (result.code == System.Net.HttpStatusCode.OK)
                    {
                        var responseJson = JsonNode.Parse(result.response!);

                        LocalStorage.Set("AccessToken", responseJson["access_token"]!.GetValue<string>());
                        LocalStorage.Set("MyUserName", responseJson["username"]!.GetValue<string>());
                        LocalStorage.Set("MyId", responseJson["id"]!.GetValue<long>().ToString());

                        Window newWindow = new TicketsWindow();
                        newWindow.Show();

                        WindowInstance.Close();
                    }
                    else
                    {
                        IncorrectAuthVisible = Visibility.Visible;
                    }
                });
            }
        }

        public RelayCommand RegistrationCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                   
                    RegistrationWindow window = new RegistrationWindow();
                    window.ShowDialog();
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
