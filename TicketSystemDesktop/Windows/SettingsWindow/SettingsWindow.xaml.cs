using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для SettingsWindows.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            AccountTab.DataContext = new SettingsWindowUserViewModel();
            UsersTab.DataContext = new SettingsWindowUsersListViewModel();
            CompaniesTab.DataContext = new SettingsWindowCompaniesViewModel();
            UserGroupsTab.DataContext = new SettingsWindowUserGroupsViewModel();
            TopicsTab.DataContext = new SettingsWindowTopicViewModel();
            StatusesTab.DataContext = new SettingsWindowStatusesViewModel();
            UserAccessGroupsTab.DataContext = new SettingsWindowUserRightsViewModel();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (((e.Source as TabControl)?.SelectedItem as TabItem)?.DataContext as ILoadableViewModel)?.Load();
        }
    }
}
