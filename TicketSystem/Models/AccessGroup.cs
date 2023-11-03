using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Models
{
    [Table("accessgroups", Schema = "suz")]
    public class AccessGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool CanSubscribe { get; set; }
        public bool CanSeeHisTickets { get; set; }
        public bool CanSeeCompanyTickets { get; set; }
        public bool CanSeeAllTickets { get; set; }
        public bool CanEditTickets { get; set; }
        public bool CanDeleteTickets { get; set; }
        public bool CanSeeServiceComments { get; set; }
        public bool CanRegisterUsers { get; set; }
        public bool CanSelectTopic { get; set; }
        public bool CanEditUsers { get; set; }
        public bool CanSelectUrgency { get; set; }
        public bool CanTakeTickets { get; set; }
        public bool CanAssignTickets { get; set; }
        public bool CanFinishTickets { get; set; }
        public bool CanMoveTickets { get; set; }

        public SendAccessGroup ToSend()
        {
            return new SendAccessGroup()
            {
                Id = Id,
                Name = Name,
                CanSubscribe = CanSubscribe,
                CanDeleteTickets = CanDeleteTickets,
                CanEditTickets = CanEditTickets,
                CanRegisterUsers = CanRegisterUsers,
                CanSeeAllTickets = CanSeeAllTickets,
                CanSeeCompanyTickets = CanSeeCompanyTickets,
                CanSeeHisTickets = CanSeeHisTickets,
                CanSeeServiceComments = CanSeeServiceComments,
                CanSelectTopic = CanSelectTopic,
                CanEditUsers = CanEditUsers,
                CanSelectUrgency = CanSelectUrgency,
                CanAssignTickets = CanAssignTickets,
                CanFinishTickets = CanFinishTickets,
                CanMoveTickets = CanMoveTickets,
                CanTakeTickets = CanTakeTickets,
            };
        }
    }

    public record struct SendAccessGroup(
        long Id,
        string Name,
        bool CanSubscribe, 
        bool CanSeeHisTickets, 
        bool CanSeeCompanyTickets, 
        bool CanSeeAllTickets, 
        bool CanEditTickets, 
        bool CanDeleteTickets, 
        bool CanSeeServiceComments, 
        bool CanRegisterUsers, 
        bool CanSelectTopic, 
        bool CanEditUsers, 
        bool CanSelectUrgency,
        bool CanTakeTickets,
        bool CanAssignTickets,
        bool CanFinishTickets,
        bool CanMoveTickets
    );
}
