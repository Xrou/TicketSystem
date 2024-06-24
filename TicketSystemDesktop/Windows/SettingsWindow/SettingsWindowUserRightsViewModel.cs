using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class SettingsWindowUserRightsViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<UserAccessGroup> UserAccessGroups { get; set; } = new ObservableCollection<UserAccessGroup>();
        public List<UserAccessGroup> SourceUserAccessGroups = new List<UserAccessGroup>();

        public RelayCommand SaveUserAccessGroupsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        Dictionary<string, object> changes = new Dictionary<string, object>();
                        foreach (var userAccessGroup in UserAccessGroups)
                        {
                            var compareUser = SourceUserAccessGroups.FirstOrDefault(u => u.Id == userAccessGroup.Id);
                            if (userAccessGroup.AccessGroupId != compareUser?.AccessGroupId)
                            {
                                changes.Add(userAccessGroup.Id.ToString(), userAccessGroup.AccessGroupId);
                            }
                        }

                        var result = HttpClient.Post("api/users/UpdateUsersAccessGroups", changes);
                        if (result.code == System.Net.HttpStatusCode.OK)
                        {
                            Logger.Log(LogStatus.Info, "SettingsWindowUserRightsViewModel.SaveUserAccessGroupsCommand",
                                $"Successfully updated access groups");
                            Load();
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "SettingsWindowUserRightsViewModel.SaveUserAccessGroupsCommand",
                                $"Cant update user access groups. Return code: {result.code}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "SettingsWindowUserRightsViewModel.SaveUserAccessGroupsCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
                    }
                });
            }
        }

        public void Load()
        {
            try
            {
                UserAccessGroups.Clear();
                SourceUserAccessGroups.Clear();

                var response = HttpClient.Get("api/Users/GetUsersAccessGroup");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = UserAccessGroup.ParseArrayFromJson(response.response);

                    foreach (var t in array)
                    {
                        UserAccessGroups.Add(t);
                        SourceUserAccessGroups.Add(t.Clone() as UserAccessGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowUserRightsViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;

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
