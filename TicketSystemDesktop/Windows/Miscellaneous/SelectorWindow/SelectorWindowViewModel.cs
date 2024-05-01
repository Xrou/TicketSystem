using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SelectorWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IDbEntity> Items { get; set; } = new ObservableCollection<IDbEntity>();
        public ObservableCollection<IDbEntity> DisplayItems { get; set; } = new ObservableCollection<IDbEntity>();
        public IDbEntity SelectedEntity { get; set; }

        private string filterText = "";
        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                DisplayItems = new ObservableCollection<IDbEntity>(Items.Where(x => (x.MainParameter.ToString()).ToLower().Contains(filterText.ToLower())));
            }
        }

        private Window window;

        public SelectorWindowViewModel(Window window, ObservableCollection<IDbEntity> items)
        {
            Items = items;
            DisplayItems = Items;
            this.window = window;
        }

        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    window.Close();
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
