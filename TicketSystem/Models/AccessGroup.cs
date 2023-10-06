namespace TicketSystem.Models
{
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

        public SendUserAccessGroupRights ToSend()
        {
            return new SendUserAccessGroupRights()
            {
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
            };
        }
    }

    public record struct SendUserAccessGroupRights(bool CanSubscribe, bool CanSeeHisTickets, bool CanSeeCompanyTickets, bool CanSeeAllTickets, bool CanEditTickets, bool CanDeleteTickets, bool CanSeeServiceComments, bool CanRegisterUsers, bool CanSelectTopic, bool CanEditUsers);
}
