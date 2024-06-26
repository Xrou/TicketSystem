﻿using System;
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
    /// Логика взаимодействия для TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : Window
    {
        public TicketWindow(Ticket ticket)
        {
            InitializeComponent();
            DataContext = new TicketWindowViewModel(ticket, this);
        }

        private void HandleCommentFileClick(object sender, MouseButtonEventArgs e)
        {
            ((TicketWindowViewModel)DataContext).FileDownloadFromCommentCommand.Execute((sender as TextBlock).Text);
        }
    }
}
