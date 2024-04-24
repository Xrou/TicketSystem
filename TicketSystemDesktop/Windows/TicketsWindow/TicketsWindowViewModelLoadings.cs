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
        public void LoadTickets()
        {
            Tickets.Clear();
            var response = HttpClient.Get("api/tickets", new KeyValuePair<string, object>[]
            {
                        new KeyValuePair<string, object>("page", PageNumber),
                        new KeyValuePair<string, object>("ticketId", FilterId),
                        new KeyValuePair<string, object>("topicId", FilterTopic?.Id),
                        new KeyValuePair<string, object>("senderUserId", FilterSenderUser?.Id),
                        new KeyValuePair<string, object>("executorUserId", FilterExecutorUser?.Id),
                        new KeyValuePair<string, object>("companyId", FilterCompany?.Id),
                        new KeyValuePair<string, object>("statusId", FilterStatus?.Id),
                        new KeyValuePair<string, object>("filterText", FilterText),
            });

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Ticket.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    Tickets.Add(t);
                }
            }
        }

        public void Load()
        {
            LoadTickets();

            FilterTopics.Clear();

            var response = HttpClient.Get("api/topics");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Topic.ParseArrayFromJson(response.response);

                foreach (var t in array)
                {
                    FilterTopics.Add(t);
                }
            }

            FilterCompanies.Clear();

            response = HttpClient.Get("api/companies");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Company.ParseArrayFromJson(response.response);

                foreach (var c in array)
                {
                    FilterCompanies.Add(c);
                }
            }

            FilterStatuses.Clear();

            response = HttpClient.Get("api/statuses");

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var array = Status.ParseArrayFromJson(response.response);

                foreach (var s in array)
                {
                    FilterStatuses.Add(s);
                }
            }
        }
    }
}
