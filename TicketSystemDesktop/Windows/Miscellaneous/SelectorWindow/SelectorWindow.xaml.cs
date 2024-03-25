using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    /// <summary>
    /// Логика взаимодействия для SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow : Window
    {
        public SelectorWindow(ObservableCollection<IDbEntity> items)
        {
            InitializeComponent();
            DataContext = new SelectorWindowViewModel(this, items);
        }

        public IDbEntity GetSelectedEntity()
        {
            return (DataContext as SelectorWindowViewModel)!.SelectedEntity;
        }
    }
}
