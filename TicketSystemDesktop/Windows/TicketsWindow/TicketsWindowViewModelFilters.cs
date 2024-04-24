using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketsWindowViewModel
    {
        public ObservableCollection<Topic> FilterTopics { get; set; } = new ObservableCollection<Topic>();
        public ObservableCollection<Company> FilterCompanies { get; set; } = new ObservableCollection<Company>();
        public ObservableCollection<Status> FilterStatuses { get; set; } = new ObservableCollection<Status>();
        
        private User? filterSenderUser;
        public User? FilterSenderUser
        {
            get
            {
                return filterSenderUser;
            }
            set
            {
                filterSenderUser = value;
                OnPropertyChanged("FilterSenderUser");
            }
        }

        private User? filterExecutorUser;
        public User? FilterExecutorUser
        {
            get
            {
                return filterExecutorUser;
            }
            set
            {
                filterExecutorUser = value;
                OnPropertyChanged("FilterExecutorUser");
            }
        }

        private Topic? filterTopic;
        public Topic? FilterTopic
        {
            get
            {
                return filterTopic;
            }
            set
            {
                filterTopic = value;
                OnPropertyChanged("FilterTopic");
            }
        }

        private Company? filterCompany;
        public Company? FilterCompany
        {
            get
            {
                return filterCompany;
            }
            set
            {
                filterCompany = value;
                OnPropertyChanged("FilterCompany");
            }
        }

        private Status? filterStatus;
        public Status? FilterStatus {
            get { return filterStatus; } 
            set { filterStatus = value; OnPropertyChanged("FilterStatus"); } 
        }

        private string? filterText;
        public string? FilterText
        {
            get { return filterText; }
            set { filterText = value; OnPropertyChanged("FilterText"); }
        }

        private long? filterId;
        public long? FilterId
        {
            get { return filterId; }
            set { filterId = value; OnPropertyChanged("FilterId"); }
        }

        private int pageNumber = 1;
        public int PageNumber
        {
            get { return pageNumber; }
            set
            {
                if (value > 0)
                {
                    pageNumber = value;
                    LoadTickets();
                    OnPropertyChanged("PageNumber");
                }
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
        public RelayCommand SelectSenderCommand
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

                        foreach (var u in array)
                        {
                            users.Add(u);
                        }
                    }

                    SelectorWindow window = new SelectorWindow(users);
                    window.ShowDialog();

                    var user = window.GetSelectedEntity() as User;

                    if (user != null)
                    {
                        FilterSenderUser = user!;
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
                    ObservableCollection<IDbEntity> users = new ObservableCollection<IDbEntity>();
                    var response = HttpClient.Get("api/users");

                    if (response.code == System.Net.HttpStatusCode.OK)
                    {
                        var array = User.ParseArrayFromJson(response.response);

                        foreach (var u in array)
                        {
                            users.Add(u);
                        }
                    }

                    SelectorWindow window = new SelectorWindow(users);
                    window.ShowDialog();

                    var user = window.GetSelectedEntity() as User;

                    if (user != null)
                    {
                        FilterExecutorUser = user!;
                    }
                });
            }
        }
        public RelayCommand ClearFilters
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    FilterTopic = null;
                    FilterCompany = null;
                    FilterSenderUser = null;
                    FilterText = null;
                    FilterId = null;
                    FilterStatus = null;
                    LoadTickets();
                });
            }
        }
        public RelayCommand ApplyFilters
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    LoadTickets();
                });
            }
        }
    }
}
