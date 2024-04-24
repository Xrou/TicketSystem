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
    public class RegistrationWindowViewModel : INotifyPropertyChanged
    {
        private string login;
        private string password;
        private string fullName;
        private string company;
        private string phone;
        private string email;
        private string code;

        public string Login { get { return login; } set { login = value; OnPropertyChanged("Login"); } }
        public string Password { get { return password; } set { password = value; OnPropertyChanged("Password"); } }
        public string FullName { get { return fullName; } set { fullName = value; OnPropertyChanged("FullName"); } }
        public string Company { get { return company; } set { company = value; OnPropertyChanged("Company"); } }
        public string Phone { get { return phone; } set { phone = value; OnPropertyChanged("Phone"); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged("Email"); } }
        public string Code { get { return code; } set { code = value; OnPropertyChanged("Code"); } }

        private Window window;

        public RegistrationWindowViewModel(Window window)
        {
            this.window = window;
        }

        public RelayCommand RegisterCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var response = HttpClient.Post("api/users/register", new Dictionary<string, object>()
                    {
                        {"login", Login},
                        {"password", Password},
                        {"fullName", FullName},
                        {"company", Company},
                        {"phoneNumber", Phone},
                        {"email", Email},
                        {"verificationCode", Code}
                    });

                    if (response.code == System.Net.HttpStatusCode.Created)
                    {
                        MessageBox.Show("Спасибо за регистрацию, вы сможете войти в учетную запись после подтверждения регистрации администратором.");
                        window.Close();
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
