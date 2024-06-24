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
    public class SettingsWindowAccessGroupRightsViewModel : INotifyPropertyChanged, ILoadableViewModel
    {
        public ObservableCollection<AccessGroup> AccessGroups { get; set; } = new ObservableCollection<AccessGroup>();
        public List<AccessGroup> SourceAccessGroups { get; set; } = new List<AccessGroup>();

        public void Load()
        {
            try
            {
                AccessGroups.Clear();
                SourceAccessGroups.Clear();

                var response = HttpClient.Get("api/AccessGroups");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = AccessGroup.ParseArrayFromJson(response.response);

                    foreach (var u in array)
                    {
                        AccessGroups.Add(u);
                        SourceAccessGroups.Add(u.Clone() as AccessGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "SettingsWindowAccessGroupRightsViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }
        }

        public RelayCommand SaveUserAccessGroupsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    try
                    {
                        Dictionary<string, object> changes = new Dictionary<string, object>();

                        for (int accessGroupId = 0; accessGroupId < AccessGroups.Count; accessGroupId++)
                        {
                            changes[(accessGroupId + 1).ToString()] = new Dictionary<string, bool>();

                            if (AccessGroups[accessGroupId].CanSubscribe != SourceAccessGroups[accessGroupId].CanSubscribe)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSubscribe", AccessGroups[accessGroupId].CanSubscribe);
                            }
                            if (AccessGroups[accessGroupId].CanSeeHisTickets != SourceAccessGroups[accessGroupId].CanSeeHisTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSeeHisTickets", AccessGroups[accessGroupId].CanSeeHisTickets);
                            }
                            if (AccessGroups[accessGroupId].CanSeeCompanyTickets != SourceAccessGroups[accessGroupId].CanSeeCompanyTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSeeCompanyTickets", AccessGroups[accessGroupId].CanSeeCompanyTickets);
                            }
                            if (AccessGroups[accessGroupId].CanSeeAllTickets != SourceAccessGroups[accessGroupId].CanSeeAllTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSeeAllTickets", AccessGroups[accessGroupId].CanSeeAllTickets);
                            }
                            if (AccessGroups[accessGroupId].CanEditTickets != SourceAccessGroups[accessGroupId].CanEditTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canEditTickets", AccessGroups[accessGroupId].CanEditTickets);
                            }
                            if (AccessGroups[accessGroupId].CanDeleteTickets != SourceAccessGroups[accessGroupId].CanDeleteTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canDeleteTickets", AccessGroups[accessGroupId].CanDeleteTickets);
                            }
                            if (AccessGroups[accessGroupId].CanSeeServiceComments != SourceAccessGroups[accessGroupId].CanSeeServiceComments)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSeeServiceComments", AccessGroups[accessGroupId].CanSeeServiceComments);
                            }
                            if (AccessGroups[accessGroupId].CanRegisterUsers != SourceAccessGroups[accessGroupId].CanRegisterUsers)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canRegisterUsers", AccessGroups[accessGroupId].CanRegisterUsers);
                            }
                            if (AccessGroups[accessGroupId].CanSelectTopic != SourceAccessGroups[accessGroupId].CanSelectTopic)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSelectTopic", AccessGroups[accessGroupId].CanSelectTopic);
                            }
                            if (AccessGroups[accessGroupId].CanEditUsers != SourceAccessGroups[accessGroupId].CanEditUsers)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canEditUsers", AccessGroups[accessGroupId].CanEditUsers);
                            }
                            if (AccessGroups[accessGroupId].CanSelectUrgency != SourceAccessGroups[accessGroupId].CanSelectUrgency)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canSelectUrgency", AccessGroups[accessGroupId].CanSelectUrgency);
                            }
                            if (AccessGroups[accessGroupId].CanTakeTickets != SourceAccessGroups[accessGroupId].CanTakeTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canTakeTickets", AccessGroups[accessGroupId].CanTakeTickets);
                            }
                            if (AccessGroups[accessGroupId].CanAssignTickets != SourceAccessGroups[accessGroupId].CanAssignTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canAssignTickets", AccessGroups[accessGroupId].CanAssignTickets);
                            }
                            if (AccessGroups[accessGroupId].CanFinishTickets != SourceAccessGroups[accessGroupId].CanFinishTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canFinishTickets", AccessGroups[accessGroupId].CanFinishTickets);
                            }
                            if (AccessGroups[accessGroupId].CanMoveTickets != SourceAccessGroups[accessGroupId].CanMoveTickets)
                            {
                                (changes[(accessGroupId + 1).ToString()] as Dictionary<string, bool>).Add("canMoveTickets", AccessGroups[accessGroupId].CanMoveTickets);
                            }
                        }

                        var result = HttpClient.Post("api/AccessGroups/UpdateAccessGroups", changes);
                        if (result.code == System.Net.HttpStatusCode.OK)
                        {
                            Logger.Log(LogStatus.Info, "SettingsWindowAccessGroupRightsViewModel.SaveUserAccessGroupsCommand",
                                $"Successfully updated access groups");
                            Load();
                        }
                        else
                        {
                            Logger.Log(LogStatus.Warning, "SettingsWindowAccessGroupRightsViewModel.SaveUserAccessGroupsCommand",
                                $"Cant update AccessGroup");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogStatus.Error, "SettingsWindowAccessGroupRightsViewModel.SaveUserAccessGroupsCommand",
                            $"{ex.Message}\n\n{ex.StackTrace}");
                        throw ex;
                    }
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