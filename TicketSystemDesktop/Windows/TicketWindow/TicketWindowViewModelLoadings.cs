using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketWindowViewModel
    {
        public void Load()
        {
            LoadComments();
        }

        public void LoadComments()
        {
            StandardComments.Clear();
            OfficialComments.Clear();
            ServiceComments.Clear();

            var response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "1") });

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var comments = Comment.ParseArrayFromJson(response.response);
                foreach(var c in comments)
                {
                    StandardComments.Add(c);
                }
            }

            response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "2") });

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var comments = Comment.ParseArrayFromJson(response.response);
                foreach (var c in comments)
                {
                    OfficialComments.Add(c);
                }
            }

            response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "3") });

            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var comments = Comment.ParseArrayFromJson(response.response);
                foreach (var c in comments)
                {
                    ServiceComments.Add(c);
                }
            }
        }
    }
}
