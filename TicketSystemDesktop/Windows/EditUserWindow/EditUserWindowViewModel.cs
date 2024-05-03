using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class EditUserWindowViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public User User { get; set; }
        public ObservableCollection<Company> Companies { get; set; } = new ObservableCollection<Company>();
        public Company UserCompany { get; set; }

        public EditUserWindowViewModel(User user)
        {
            User = user;
            Load();
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var data = new Dictionary<string, object>() {
                        { "fullName", User.FullName },
                        { "companyId", UserCompany.Id },
                        { "phoneNumber", User.PhoneNumber },
                        { "email", User.Email },
                        { "telegram", User.TelegramId },
                        { "pcName", User.PCName },
                    };

                    var result = HttpClient.Post($"api/users/{User.Id}", data);
                });
            }
        }


        public void Load()
        {
            Companies.Clear();

            var response = HttpClient.Get("api/companies");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Company.ParseArrayFromJson(response.response);

                foreach (var c in array)
                {
                    Companies.Add(c);
                }
            }

            UserCompany = Companies.First(x => x.Id == User.CompanyId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
