using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketsWindowViewModel
    {
        public void Load()
        {
            Tickets.Clear();
            var response = HttpClient.Get("api/tickets");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Ticket.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    Tickets.Add(t);
                }
            }
        }
    }
}
