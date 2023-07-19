namespace TicketSystem.Models
{
    public class User
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public long AccessGroupId { get; set; }

        public List<Subscription> Subscriptions { get; set; } = new();
        public List<UserGroup> UserGroups { get; set; } = new();

        virtual public AccessGroup AccessGroup { get; set; } = null!;
        virtual public Company Company { get; set; } = null!;

        public SendUser ToSend()
        {
            return new SendUser(Id, Name, Company.Name, AccessGroup.Name);
        }
    }

    public record struct AuthorizeUser(string Login, string Password);
    public record struct SendUser(long Id, string Name, string companyName, string accessGroupName);

}
