using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("usergroups")]
    public class UserGroup
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public List<User> Users { get; set; } = new();
        public List<Company> Companies { get; set; } = new();

        public SendUserGroup ToSend()
        {
            return new SendUserGroup() { Id = Id, Name = Name, Users = Users.Select(u => u.ToSend()).ToList(), Companies = Companies.Select(c => c.ToSend()).ToList() };
        }

        public SendUserGroupShort ToSendShort()
        {
            return new SendUserGroupShort() { Id = Id, Name = Name };
        }
    }

    public record struct SendUserGroup(long Id, string Name, List<SendUser> Users, List<SendCompany> Companies);
    public record struct SendUserGroupShort(long Id, string Name);
}
