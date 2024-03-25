using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public class UserAccessGroup : INotifyPropertyChanged, IDbEntity, ICloneable
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
        public int AccessGroupId
        {
            get
            {
                return accessGroupId;
            }
            set
            {
                if (accessGroupId != value) // recursion protection
                {
                    accessGroupId = value;

                    switch (accessGroupId)
                    {
                        case 1:
                            IsUserAccessGroup = true;
                            IsSupervisorAccessGroup = false;
                            IsLine1AccessGroup = false;
                            IsLine2AccessGroup = false; // govnokod
                            IsAdminAccessGroup = false;
                            break;
                        case 2:
                            IsSupervisorAccessGroup = true;
                            IsUserAccessGroup = false;
                            IsLine1AccessGroup = false;
                            IsLine2AccessGroup = false;
                            IsAdminAccessGroup = false;
                            break;
                        case 3:
                            IsLine1AccessGroup = true;
                            IsUserAccessGroup = false;
                            IsSupervisorAccessGroup = false;
                            IsLine2AccessGroup = false;
                            IsAdminAccessGroup = false;
                            break;
                        case 4:
                            IsLine2AccessGroup = true;
                            IsUserAccessGroup = false;
                            IsSupervisorAccessGroup = false;
                            IsLine1AccessGroup = false;
                            IsAdminAccessGroup = false;
                            break;
                        case 5:
                            IsAdminAccessGroup = true;
                            IsUserAccessGroup = false;
                            IsSupervisorAccessGroup = false;
                            IsLine1AccessGroup = false;
                            IsLine2AccessGroup = false;
                            break;
                    }

                    OnPropertyChanged("AccessGroupId");
                }
            }
        }
        public bool IsUserAccessGroup
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
                    AccessGroupId = 1;
                }
                
                OnPropertyChanged("IsUserAccessGroup");
            }
        }
        public bool IsSupervisorAccessGroup
        {
            get
            {
                return isSupervisorAccessGroup;
            }
            set
            {
                isSupervisorAccessGroup = value;
                
                if (value == true)
                {
                    AccessGroupId = 2;
                }
                
                OnPropertyChanged("IsSupervisorAccessGroup");
            }
        }
        public bool IsLine1AccessGroup
        {
            get
            {
                return isLine1AccessGroup;
            }
            set
            {
                isLine1AccessGroup = value;
                
                if (value == true)
                {
                    AccessGroupId = 3;
                }

                OnPropertyChanged("IsLine1AccessGroup");
            }
        }
        public bool IsLine2AccessGroup
        {
            get
            {
                return isLine2AccessGroup;
            }
            set
            {
                isLine2AccessGroup = value;
                
                if (value == true)
                {
                    AccessGroupId = 4;
                }

                OnPropertyChanged("IsLine2AccessGroup");
            }
        }
        public bool IsAdminAccessGroup
        {
            get
            {
                return isAdminAccessGroup;
            }
            set
            {
                isAdminAccessGroup = value;
                
                if (value == true)
                {
                    AccessGroupId = 5;
                }

                OnPropertyChanged("IsAdminAccessGroup");
            }
        }

        public object MainParameter { get { return fullName; } }

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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
