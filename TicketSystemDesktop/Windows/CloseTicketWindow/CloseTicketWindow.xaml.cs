using System;
using System.Collections.Generic;
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

namespace TicketSystemDesktop
{
    /// <summary>
    /// Логика взаимодействия для CloseTicketWindow.xaml
    /// </summary>
    public partial class CloseTicketWindow : Window
    {
        public CloseTicketWindow(long ticketId)
        {
            InitializeComponent();
            DataContext = new CloseTicketWindowViewModel(this, ticketId);
        }

        public bool GetResult()
        {
            return (DataContext as CloseTicketWindowViewModel)!.TicketClosed;
        }
    }
}
