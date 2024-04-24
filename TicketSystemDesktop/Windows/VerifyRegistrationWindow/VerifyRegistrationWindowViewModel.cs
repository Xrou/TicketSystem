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
    public class VerifyRegistrationWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        private string fullName;
        private string possibleCompany;
        private string phone;
        private string email;

        public string FullName { get { return fullName; } set { fullName = value; OnPropertyChanged("FullName"); } }
        public string Phone { get { return phone; } set { phone = value; OnPropertyChanged("Phone"); } }
        public string Email { get { return email; } set { email = value; OnPropertyChanged("Email"); } }

        public ObservableCollection<Company> AvailableCompanies { get; set; } = new ObservableCollection<Company>();
        private Company? selectedCompany;
        public Company? SelectedCompany { get { return selectedCompany; } set { selectedCompany = value; OnPropertyChanged("SelectedCompany"); } }

        private Window window;
        private Ticket ticket;

        public bool Result = false;

        public VerifyRegistrationWindowViewModel(Window window, Ticket ticket)
        {
            this.window = window;
            this.ticket = ticket;
            Load();
        }

        public void Load()
        {
            AvailableCompanies.Clear();

            var response = HttpClient.Get("api/companies");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Company.ParseArrayFromJson(response.response);

                foreach (var c in array)
                {
                    AvailableCompanies.Add(c);
                }
            }

            response = HttpClient.Get($"api/users/{ticket.UserId}");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var userObject = JsonNode.Parse(response.response).AsObject();

                var user = User.ParseFromJson(userObject);

                FullName = user.FullName;
                Phone = user.PhoneNumber;
                Email = user.Email;
            }
        }

        public RelayCommand RegisterCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var response = HttpClient.Post("api/users/confirmRegistration", new Dictionary<string, object>()
                    {
                        {"ticketId", ticket.Id.ToString()},
                        {"fullName", FullName},
                        {"companyId", SelectedCompany.Id},
                        {"phone", Phone},
                        {"email", Email},
                    });

                    if (response.code == System.Net.HttpStatusCode.OK)
                    {
                        window.Close();
                        Result = true;
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
