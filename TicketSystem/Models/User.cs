﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("users", Schema = "suz")]
    public class User
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public long AccessGroupId { get; set; }

        public bool CanLogin { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long Telegram { get; set; }


        virtual public List<Subscription> Subscriptions { get; set; } = new();
        virtual public List<UserGroup> UserGroups { get; set; } = new();

        virtual public AccessGroup AccessGroup { get; set; } = null!;
        virtual public Company Company { get; set; } = null!;

        public SendUser ToSend()
        {
            return new SendUser(Id, Name, Company.Id, Company.Name, Company.ShortName, AccessGroup.Name, FullName, Email, PhoneNumber, Telegram);
        }

        public SendUserAccessGroup ToSendUserAccessGroup()
        {
            return new SendUserAccessGroup(Id, FullName, AccessGroup.Id);
        }

        public SendUserGroups ToSendUserGroups()
        {
            return new SendUserGroups(Id, FullName, Company.Name, UserGroups.Select(u => u.ToSendShort()).ToList());
        }
    }

    public record struct AuthorizeUser(string Login, string Password);
    public record struct SendUser(long Id, string Name, long CompanyId, string companyName, string companyShortName, string accessGroupName, string fullName, string email, string phoneNumber, long telegram);
    public record struct SendUserAccessGroup(long Id, string FullName, long AccessGroupId);
    public record struct SendUserGroups(long Id, string FullName, string companyName, List<SendUserGroupShort> UserGroups);
}
