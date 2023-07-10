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
    }
}
