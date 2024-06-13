using System.ComponentModel.DataAnnotations.Schema;

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
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PCName { get; set; }
        public long? Telegram { get; set; }

        public bool CanLogin { get; set; }

        virtual public List<Subscription> Subscriptions { get; set; } = new();
        virtual public List<UserGroup> UserGroups { get; set; } = new();

        virtual public AccessGroup AccessGroup { get; set; } = null!;
        virtual public Company Company { get; set; } = null!;

        public SendUser ToSend()
        {
            return new SendUser(Id, Name, Company.Id, Company.Name, Company.ShortName, PCName, AccessGroup.Name, FullName, Email, PhoneNumber, Telegram);
        }

        public SendUserAccessGroup ToSendUserAccessGroup()
        {
            return new SendUserAccessGroup(Id, FullName, AccessGroup.Id);
        }

        public SendUserGroups ToSendUserGroups()
        {
            return new SendUserGroups(Id, FullName, Company.Name, UserGroups.Select(u => u.ToSendShort()).ToList());
        }

        public override string ToString()
        {
            return $"User id:{Id}, company:{CompanyId}, name:{Name}, password hash:{PasswordHash}, access group:{AccessGroupId}, full name:{FullName}, email:{Email}, phone:{PhoneNumber}, pc name:{PCName}, telegram:{Telegram}, can login:{CanLogin}";
        }
    }

    public record struct AuthorizeUser(string Login, string Password);
    public record struct SendUser(long Id, string Name, long CompanyId, string CompanyName, string CompanyShortName, string PCName, string AccessGroupName, string FullName, string Email, string PhoneNumber, long? Telegram);
    public record struct SendUserAccessGroup(long Id, string FullName, long AccessGroupId);
    public record struct SendUserGroups(long Id, string FullName, string CompanyName, List<SendUserGroupShort> UserGroups);
}
