using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TicketSystemDesktop.Miscellaneous;
using TicketSystemDesktop.Models;

namespace TicketSystemDesktop
{
    public partial class TicketsWindowViewModel
    {
        public void LoadTickets()
        {
            try
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
                    Logger.Log(LogStatus.Info, "TicketsWindowViewModel.LoadTickets",
                        $"Successfully loaded tickets");
                }
                else
                {
                    Logger.Log(LogStatus.Info, "TicketsWindowViewModel.LoadTickets",
                        $"Cant load tickets with data:\npage={PageNumber}\nticketId={FilterId}\ntopicId={FilterTopic?.Id}\nsenderUserId={FilterSenderUser?.Id}\n" +
                        $"executorUserId={FilterExecutorUser?.Id}\ncompanyId={FilterCompany}\nstatusId={FilterStatus}\nfilterText={FilterText}\n\nReturn code: {response.code}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.LoadTickets",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }
        }

        public void Load()
        {
            try
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
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }

            try
            {
                FilterCompanies.Clear();

                var response = HttpClient.Get("api/companies");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Company.ParseArrayFromJson(response.response);

                    foreach (var c in array)
                    {
                        FilterCompanies.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }

            try
            {
                FilterStatuses.Clear();

                var response = HttpClient.Get("api/statuses");

                if (response.code == System.Net.HttpStatusCode.OK)
                {
                    var array = Status.ParseArrayFromJson(response.response);

                    foreach (var s in array)
                    {
                        FilterStatuses.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogStatus.Error, "TicketsWindowViewModel.Load",
                    $"{ex.Message}\n\n{ex.StackTrace}");
                throw ex;
            }
        }
    }
}
