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
using TicketSystemDesktop.Miscellaneous;

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
                    try
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
                            Logger.Log(LogStatus.Info, "LoginWindowViewModel.AuthorizeCommand",
                                $"Successfully authorized user");
                        }
                        else
                        {
                            IncorrectAuthVisible = Visibility.Visible;
                            Logger.Log(LogStatus.Warning, "LoginWindowViewModel.AuthorizeCommand",
                                $"Cant authorize user with data:\nlogin={Username}\npassword={Password}\n\nReturned code:{result.code}");

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "LoginWindowViewModel.AuthorizeCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
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
