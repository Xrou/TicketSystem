using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class UserAccessGroup : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string fullName;
        private int accessGroupId;
        private bool isUserAccessGroup;
        private bool isSupervisorAccessGroup;
        private bool isLine1AccessGroup;
        private bool isLine2AccessGroup;
        private bool isAdminAccessGroup;

        public long Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        public string FullName { get { return fullName; } set { fullName = value; OnPropertyChanged("Name"); } }
        public int AccessGroupId { get { return accessGroupId; } set { accessGroupId = value; OnPropertyChanged("AccessGroupId"); } }
        private bool IsUserAccessGroup
        {
            get
            {
                return isUserAccessGroup;
            }
            set
            {
                isUserAccessGroup = value; 
                if (value == true)
                {
                    isUserAccessGroup = false;
                }
                OnPropertyChanged("");
            }
        }
        private bool IsSupervisorAccessGroup { get { return isSupervisorAccessGroup; } set { isSupervisorAccessGroup = value; OnPropertyChanged(""); } }
        private bool IsLine1AccessGroup { get { return isLine1AccessGroup; } set { isLine1AccessGroup = value; OnPropertyChanged(""); } }
        private bool IsLine2AccessGroup { get { return isLine2AccessGroup; } set { isLine2AccessGroup = value; OnPropertyChanged(""); } }
        private bool IsAdminAccessGroup { get { return isAdminAccessGroup; } set { isAdminAccessGroup = value; OnPropertyChanged(""); } }

        public static UserAccessGroup ParseFromJson(JsonObject parseObject)
        {
            var id = parseObject["id"].GetValue<long>();
            var fullName = parseObject["fullName"].GetValue<string>();
            var accessGroupId = parseObject["accessGroupId"].GetValue<int>();

            return new UserAccessGroup() { Id = id, FullName = fullName, AccessGroupId = accessGroupId };
        }

        public static List<UserAccessGroup> ParseArrayFromJson(string response)
        {
            List<UserAccessGroup> parsed = new List<UserAccessGroup>();
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
}
