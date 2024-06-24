using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowUsersListViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Company> AvailableCompanies { get; set; } = new ObservableCollection<Company>();

        private string? filterLogin;
        public string? FilterLogin { get { return filterLogin; } set { filterLogin = value; OnPropertyChanged("FilterLogin"); } }

        private string? filterName;
        public string? FilterName { get { return filterName; } set { filterName = value; OnPropertyChanged("FilterName"); } }

        private string? filterPhone;
        public string? FilterPhone { get { return filterPhone; } set { filterPhone = value; OnPropertyChanged("FilterPhone"); } }

        private string? filterPCName;
        public string? FilterPCName { get { return filterPCName; } set { filterPCName = value; OnPropertyChanged("FilterPCName"); } }

        private Company? filterCompany;
        public Company? FilterCompany { get { return filterCompany; } set { filterCompany = value; OnPropertyChanged("FilterCompany"); } }

        private int pageNumber = 1;
        public int PageNumber
        {
            get { return pageNumber; }
            set
            {
                if (value > 0)
                {
                    pageNumber = value;
                    LoadUsers();
                    OnPropertyChanged("PageNumber");
                }
            }
        }

        private void LoadUsers()
        {
            try
            {
                Users.Clear();

                var response = HttpClient.Get("api/users", new KeyValuePair<string, object>[]
                {
                new KeyValuePair<string, object>("page", pageNumber),
                new KeyValuePair<string, object>("name", filterName),
                new KeyValuePair<string, object>("login", filterLogin),
                new KeyValuePair<string, object>("phone", filterPhone),
                new KeyValuePair<string, object>("companyId", FilterCompany?.Id),
                });

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = User.ParseArrayFromJson(response.response);

                    foreach (var u in array)
                    {
                        Users.Add(u);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowUsersListViewModel.LoadUsers",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }
        }

        public void Load()
        {
            try
            {
                LoadUsers();

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
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowUsersListViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;

            }
        }

        public RelayCommand UsersListDoubleClickCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var user = obj as User;

                    EditUserWindow editUserWindow = new EditUserWindow(user);
                    editUserWindow.ShowDialog();
                });
            }
        }

        public RelayCommand ClearFiltersCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    FilterCompany = null;
                    FilterLogin = null;
                    FilterPCName = null;
                    FilterPhone = null;
                    LoadUsers();
                });
            }
        }

        public RelayCommand ApplyFiltersCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    LoadUsers();
                });
            }
        }

        public RelayCommand PageRightCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    PageNumber++;
                });
            }
        }

        public RelayCommand PageLeftCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    PageNumber--;
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
