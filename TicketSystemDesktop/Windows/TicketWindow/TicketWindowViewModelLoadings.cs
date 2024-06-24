using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Miscellaneous;
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
            try
            {
                var response = HttpClient.Get($"api/tickets/{Ticket.Id}");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var ticketObject = JsonNode.Parse(response.response).AsObject();

                    Ticket = Ticket.ParseFromJson(ticketObject);

                    IsRegisterTicket = Ticket.TicketType == 2;

                    ItsMyTicket = Ticket.ExecutorUserId == long.Parse(LocalStorage.Get("MyId")!.ToString());
                    IsTicketClosed = Ticket.Status == "Завершена";
                    Logger.Log(LogStatus.Info, "TicketsWindowViewModel.LoadTicket",
                                 $"Successfully loaded ticket");
                }
                else
                {
                    Logger.Log(LogStatus.Warning, "TicketsWindowViewModel.LoadTicket",
                        $"Cant load ticket with id={Ticket.Id}\n\nReturn code: {response.code}");

                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.LoadTicket",
                    $"{ex.Message}\n\n{ex.StackTrace}");

            }
        }

        public void LoadComments()
        {
            StandardComments.Clear();
            OfficialComments.Clear();
            ServiceComments.Clear();

            try
            {
                var response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "1") });

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var comments = Comment.ParseArrayFromJson(response.response);
                    foreach (var c in comments)
                    {
                        StandardComments.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.LoadComments",
                    $"{ex.Message}\n\n{ex.StackTrace}");
            }

            try
            {
                var response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "2") });

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var comments = Comment.ParseArrayFromJson(response.response);
                    foreach (var c in comments)
                    {
                        OfficialComments.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.LoadComments",
                    $"{ex.Message}\n\n{ex.StackTrace}");
            }

            try
            {
                var response = HttpClient.Get("api/comments", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ticketId", Ticket.Id), new KeyValuePair<string, object>("commentType", "3") });

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var comments = Comment.ParseArrayFromJson(response.response);
                    foreach (var c in comments)
                    {
                        ServiceComments.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.LoadComments",
                    $"{ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }
}
