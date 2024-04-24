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
            LoadTicket();
            LoadComments();
        }

        public void LoadTicket()
        {
            var response = HttpClient.Get($"api/tickets/{Ticket.Id}");
        
            if (response.code == System.Net.HttpStatusCode.OK)
            {
                var ticketObject = JsonNode.Parse(response.response).AsObject();

                Ticket = Ticket.ParseFromJson(ticketObject);

                IsRegisterTicket = Ticket.TicketType == 2;

                ItsMyTicket = Ticket.ExecutorUserId == long.Parse(LocalStorage.Get("MyId")!.ToString());
                IsTicketClosed = Ticket.Status == "Завершена";
            }
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
