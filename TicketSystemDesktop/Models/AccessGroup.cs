using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Reflection;

namespace TicketSystemDesktop.Models
{
    public class AccessGroup : INotifyPropertyChanged, IDbEntity, ICloneable
    {
        private long id;
        private string name;
        private bool canSubscribe;
        private bool canSeeHisTickets;
        private bool canSeeCompanyTickets;
        private bool canSeeAllTickets;
        private bool canEditTickets;
        private bool canDeleteTickets;
        private bool canSeeServiceComments;
        private bool canRegisterUsers;
        private bool canSelectTopic;
        private bool canEditUsers;
        private bool canSelectUrgency;
        private bool canTakeTickets;
        private bool canAssignTickets;
        private bool canFinishTickets;
        private bool canMoveTickets;


        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public bool CanSubscribe { get { return canSubscribe; } set { canSubscribe = value; OnPropertyChanged("CanSubscribe"); } }
        public bool CanSeeHisTickets { get { return canSeeHisTickets; } set { canSeeHisTickets = value; OnPropertyChanged("CanSeeHisTickets"); } }
        public bool CanSeeCompanyTickets { get { return canSeeCompanyTickets; } set { canSeeCompanyTickets = value; OnPropertyChanged("CanSeeCompanyTickets"); } }
        public bool CanSeeAllTickets { get { return canSeeAllTickets; } set { canSeeAllTickets = value; OnPropertyChanged("CanSeeAllTickets"); } }
        public bool CanEditTickets { get { return canEditTickets; } set { canEditTickets = value; OnPropertyChanged("CanEditTickets"); } }
        public bool CanDeleteTickets { get { return canDeleteTickets; } set { canDeleteTickets = value; OnPropertyChanged("CanDeleteTickets"); } }
        public bool CanSeeServiceComments { get { return canSeeServiceComments; } set { canSeeServiceComments = value; OnPropertyChanged("CanSeeServiceComments"); } }
        public bool CanRegisterUsers { get { return canRegisterUsers; } set { canRegisterUsers = value; OnPropertyChanged("CanRegisterUsers"); } }
        public bool CanSelectTopic { get { return canSelectTopic; } set { canSelectTopic = value; OnPropertyChanged("CanSelectTopic"); } }
        public bool CanEditUsers { get { return canEditUsers; } set { canEditUsers = value; OnPropertyChanged("CanEditUsers"); } }
        public bool CanSelectUrgency { get { return canSelectUrgency; } set { canSelectUrgency = value; OnPropertyChanged("CanSelectUrgency"); } }
        public bool CanTakeTickets { get { return canTakeTickets; } set { canTakeTickets = value; OnPropertyChanged("CanTakeTickets"); } }
        public bool CanAssignTickets { get { return canAssignTickets; } set { canAssignTickets = value; OnPropertyChanged("CanAssignTickets"); } }
        public bool CanFinishTickets { get { return canFinishTickets; } set { canFinishTickets = value; OnPropertyChanged("CanFinishTickets"); } }
        public bool CanMoveTickets { get { return canMoveTickets; } set { canMoveTickets = value; OnPropertyChanged("CanMoveTickets"); } }

        public object MainParameter { get { return name; } }

        public static AccessGroup ParseFromJson(JsonObject ticket)
        {
            var Id = ticket["id"]!.GetValue<long>();
            var Name = ticket["name"]!.GetValue<string>();
            var CanSubscribe = ticket["canSubscribe"]!.GetValue<bool>();
            var CanSeeHisTickets = ticket["canSeeHisTickets"]!.GetValue<bool>();
            var CanSeeCompanyTickets = ticket["canSeeCompanyTickets"]!.GetValue<bool>();
            var CanSeeAllTickets = ticket["canSeeAllTickets"]!.GetValue<bool>();
            var CanEditTickets = ticket["canEditTickets"]!.GetValue<bool>();
            var CanDeleteTickets = ticket["canDeleteTickets"]!.GetValue<bool>();
            var CanSeeServiceComments = ticket["canSeeServiceComments"]!.GetValue<bool>();
            var CanRegisterUsers = ticket["canRegisterUsers"]!.GetValue<bool>();
            var CanSelectTopic = ticket["canSelectTopic"]!.GetValue<bool>();
            var CanEditUsers = ticket["canEditUsers"]!.GetValue<bool>();
            var CanSelectUrgency = ticket["canSelectUrgency"]!.GetValue<bool>();
            var CanTakeTickets = ticket["canTakeTickets"]!.GetValue<bool>();
            var CanAssignTickets = ticket["canAssignTickets"]!.GetValue<bool>();
            var CanFinishTickets = ticket["canFinishTickets"]!.GetValue<bool>();
            var CanMoveTickets = ticket["canMoveTickets"]!.GetValue<bool>();

            return new AccessGroup
            {
                Id = Id,
                Name = Name,
                CanSubscribe = CanSubscribe,
                CanSeeHisTickets = CanSeeHisTickets,
                CanSeeCompanyTickets = CanSeeCompanyTickets,
                CanSeeAllTickets = CanSeeAllTickets,
                CanEditTickets = CanEditTickets,
                CanDeleteTickets = CanDeleteTickets,
                CanSeeServiceComments = CanSeeServiceComments,
                CanRegisterUsers = CanRegisterUsers,
                CanSelectTopic = CanSelectTopic,
                CanEditUsers = CanEditUsers,
                CanSelectUrgency = CanSelectUrgency,
                CanTakeTickets = CanTakeTickets,
                CanAssignTickets = CanAssignTickets,
                CanFinishTickets = CanFinishTickets,
                CanMoveTickets = CanMoveTickets
            };
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static List<AccessGroup> ParseArrayFromJson(string response)
        {
            List<AccessGroup> parsed = new List<AccessGroup>();
            var array = JsonArray.Parse(response);

            foreach (var t in array.AsArray())
            {
                var obj = t.AsObject();
                parsed.Add(ParseFromJson(obj));
            }

            return parsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class RightAtAccessGroup : INotifyPropertyChanged
    {
        private long id;
        private string name;
        private bool atUser;
        private bool atSupervisor;
        private bool atLine1;
        private bool atLine2;
        private bool atAdmin;

        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public bool AtUser { get { return atUser; } set { atUser = value; OnPropertyChanged("AtUser"); } }
        public bool AtSupervisor { get { return atSupervisor; } set { atSupervisor = value; OnPropertyChanged("AtSupervisor"); } }
        public bool AtLine1 { get { return atLine1; } set { atLine1 = value; OnPropertyChanged("AtLine1"); } }
        public bool AtLine2 { get { return atLine2; } set { atLine2 = value; OnPropertyChanged("AtLine2"); } }
        public bool AtAdmin { get { return atAdmin; } set { atAdmin = value; OnPropertyChanged("AtAdmin"); } }

        public RightAtAccessGroup(AccessGroup accessGroupUser,
            AccessGroup accessGroupSupervisor,
            AccessGroup accessGroupLine1,
            AccessGroup accessGroupLine2,
            AccessGroup accessGroupAdmin)
        {
            Id = accessGroupUser.Id;
            Name = accessGroupUser.Name;
            AtUser = (bool)typeof(AccessGroup)!.GetProperty(accessGroupUser.Name)!.GetValue(accessGroupUser, null)!;
            AtSupervisor = (bool)typeof(AccessGroup)!.GetProperty(accessGroupUser.Name)!.GetValue(accessGroupSupervisor, null)!;
            AtLine1 = (bool)typeof(AccessGroup)!.GetProperty(accessGroupUser.Name)!.GetValue(accessGroupLine1, null)!;
            AtLine2 = (bool)typeof(AccessGroup)!.GetProperty(accessGroupUser.Name)!.GetValue(accessGroupLine2, null)!;
            AtAdmin = (bool)typeof(AccessGroup)!.GetProperty(accessGroupUser.Name)!.GetValue(accessGroupAdmin, null)!;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}