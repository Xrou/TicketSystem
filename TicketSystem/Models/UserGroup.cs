using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("usergroups", Schema = "suz")]
    public class UserGroup
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        virtual public List<User> Users { get; set; } = new();
        virtual public List<Company> Companies { get; set; } = new();
        virtual public List<Topic> Topics { get; set; } = new();

        public SendUserGroup ToSend()
        {
            return new SendUserGroup() { Id = Id, Name = Name, Users = Users.Select(u => u.ToSend()).ToList(), Companies = Companies.Select(c => c.ToSend()).ToList(), Topics = Topics.Select(c => c.ToSend()).ToList() };
        }

        public SendUserGroupShort ToSendShort()
        {
            return new SendUserGroupShort() { Id = Id, Name = Name };
        }

        public override string ToString()
        {
            return $"UserGroup id:{Id}, name:{Name}";
        }
    }

    public record struct SendUserGroup(long Id, string Name, List<SendUser> Users, List<SendCompany> Companies, List<SendTopic> Topics);
    public record struct SendUserGroupShort(long Id, string Name);
}
