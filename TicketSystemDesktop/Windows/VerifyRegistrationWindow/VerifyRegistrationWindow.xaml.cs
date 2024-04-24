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
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    /// <summary>
    /// Логика взаимодействия для VerifyRegistrationWindow.xaml
    /// </summary>
    public partial class VerifyRegistrationWindow : Window
    {
        public VerifyRegistrationWindow(Ticket ticket)
        {
            InitializeComponent();
            DataContext = new VerifyRegistrationWindowViewModel(this, ticket);
        }

        public bool GetResult()
        {
            return (DataContext as VerifyRegistrationWindowViewModel)!.Result;
        }
    }
}
