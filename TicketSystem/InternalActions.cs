﻿using TicketSystem.Models;

namespace TicketSystem
{
    public static class InternalActions
    {
        public static bool CanUserAccessTicket(User user, Ticket ticket)
        {
            return
                (user.AccessGroup.CanSeeHisTickets && user.Id == ticket.UserId) ||
                (user.AccessGroup.CanSeeCompanyTickets && user.CompanyId == ticket.User.CompanyId);
        }

        public static User? SelectUserGroupFromContext(HttpContext httpContext, Database context)
        {
            if (httpContext.User.Identity == null)
                return null;

            User? user = context.Users.FirstOrDefault(u => u.Name == httpContext.User.Identity.Name);

            if (user == null)
                return null;

            return user!;
        }
    }
}