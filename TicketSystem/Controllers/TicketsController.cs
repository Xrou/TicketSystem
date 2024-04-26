using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly Database context;

        public TicketsController(Database context)
        {
            this.context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<List<SendTicket>>> GetTicketItems(int? page,
            long? ticketId = null, long? topicId = null, long? senderUserId = null,
            long? executorUserId = null, long? companyId = null, long? statusId = null,
            string? filterText = null)
        {
            if (page < 0)
            {
                return BadRequest("Page must be non negative number");
            }

            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
            {
                return Problem("No user instance", statusCode: 500);
            }

            if (page == null)
                page = 1;

            IQueryable<Ticket> tickets = context.Tickets;

            if (ticketId != null)
            {
                tickets = tickets.Where(x => x.Id == ticketId);
            }
            else
            {
                if (topicId != null)
                    tickets = tickets.Where(x => x.TopicId == topicId);

                if (senderUserId != null)
                    tickets = tickets.Where(x => x.SenderUser.Id == senderUserId);

                if (executorUserId != null)
                    tickets = tickets.Where(x => x.ExecutorId == executorUserId);

                if (companyId != null)
                    tickets = tickets.Where(x => x.SenderUser.CompanyId == companyId);
                
                if (statusId != null)
                    tickets = tickets.Where(x => x.Status.Id == statusId);

                if (filterText != null)
                    tickets = tickets.Where(x => x.Text.ToLower().Contains(filterText));
            }

            var filteredTickets = tickets
                .OrderByDescending(x => x.Id)
                .Where(x =>
                    (x.Finished && user.Id == x.UserId) ||
                    (user.AccessGroup.CanSeeAllTickets) ||
                    (user.AccessGroup.CanSeeHisTickets && user.Id == x.UserId) ||
                    (user.AccessGroup.CanSeeCompanyTickets && user.CompanyId == x.User.CompanyId))
                .Select(x => x.ToSend());

            if (page != 0)
            {
                return filteredTickets
                    .Page((int)page, 5)
                    .ToList();
            }
            else
            {
                return filteredTickets
                    .ToList();
            }
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendTicket>> GetTicket(long id)
        {
            var ticket = await context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (InternalActions.CanUserAccessTicket(user, ticket))
                return ticket.ToSend();

            return Forbid();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        protected internal static async Task<Ticket?> CreateTicket(Database context, HttpContext http, PostTicket ticket, long senderId, bool registration = false)
        {
            User? user = InternalActions.SelectUserFromContext(http, context);

            if (!registration && user == null)
                return null;

            DateTime ticketDate = DateTime.Now;

            TicketType ticketType = TicketType.Standard;

            if (registration)
                ticketType = TicketType.Registration;

            if (!registration && !user.AccessGroup.CanSelectTopic && ticket.TopicId != 1)
                return null;

            Ticket newTicket = new Ticket() { Type = ticketType, Date = ticketDate, Text = ticket.Text, DeadlineTime = ticketDate + new TimeSpan(3, 0, 0), SenderId = senderId, UserId = ticket.UserId, Urgency = (Urgency)ticket.Urgency, TopicId = ticket.TopicId, StatusId = 1 };

            context.Tickets.Add(newTicket);
            await context.SaveChangesAsync(); // не удалять, нужно для нормальной работы с айдишниками. сначала сохрани тикет - потом работай с ним

            if (ticket.Files != null)
            {
                foreach (var file in ticket.Files)
                {
                    newTicket.Files.Add(file);
                }
            }

            await context.SaveChangesAsync();

            return newTicket;
        }

        [HttpPost]
        public async Task<IActionResult> PostTicket(PostTicket ticket)
        {
            try
            {
                User? user = InternalActions.SelectUserFromContext(HttpContext, context);

                if (user == null)
                    return Problem("No user instance", statusCode: 500);


                if (!user.CanLogin)
                {
                    if (context.Tickets.Count(x => x.User == user) > 2)
                    {
                        return Forbid();
                    }
                }


                if (ticket.UserId == 0)
                    ticket.UserId = user.Id;

                Ticket? newTicket = await CreateTicket(context, HttpContext, ticket, user.Id);

                if (newTicket == null)
                    return BadRequest();

                return Created(new Uri("https://localhost:7177/api/tickets"), newTicket.Id);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Source}: {ex.Message} {ex.StackTrace}");
                return Problem();
            }
        }

        // api/tickets/subscribe/3
        [HttpGet("subscribe/{ticketId}")]
        public async Task<IActionResult> Subscribe(long ticketId)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (!context.Tickets.Any(t => t.Id == ticketId))
            {
                return NotFound();
            }

            if (!InternalActions.CanUserAccessTicket(user, context.Tickets.First(t => t.Id == ticketId)))
                return Forbid();

            Subscription? alreadyCreatedSubscription = context.Subscriptions.FirstOrDefault(s => s.TicketId == ticketId && s.UserId == user.Id);

            if (alreadyCreatedSubscription != null)
                return Created(new Uri("https://localhost:7177/api/subscribe/"), alreadyCreatedSubscription.Id);

            var newSubscription = new Subscription() { TicketId = ticketId, UserId = user.Id };

            context.Subscriptions.Add(newSubscription);
            await context.SaveChangesAsync();

            long subscriptionId = context.Subscriptions.First(s => s.TicketId == ticketId && s.UserId == user.Id).Id;

            return Created(new Uri("https://localhost:7177/api/subscribe/"), subscriptionId);
        }

        // api/tickets/subscribed/3
        [HttpGet("subscribed/{ticketId}")]
        public async Task<ActionResult<string>> Subscribed(long ticketId)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (!context.Tickets.Any(t => t.Id == ticketId))
            {
                return NotFound();
            }

            if (!InternalActions.CanUserAccessTicket(user, context.Tickets.First(t => t.Id == ticketId)))
                return Forbid();

            List<long> subscribedUsersIds = context.Subscriptions.Where(s => s.TicketId == ticketId).Select(s => s.UserId).ToList();

            return JsonConvert.SerializeObject(subscribedUsersIds);
        }

        //api/tickets/assignTicket
        [HttpPost("assignTicket")]
        public async Task<IActionResult> AssignTicket([FromBody] JsonObject json)
        {
            long userId;
            long ticketId;

            if (!(
                json.ContainsKey("userId") && long.TryParse(json["userId"]!.GetValue<string>(), out userId) &&
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId)))
            {
                return BadRequest();
            }

            var updateTicket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (updateTicket == null)
            {
                return NotFound();
            }

            if (!context.Users.Any(u => u.Id == userId))
            {
                return NotFound();
            }

            updateTicket.ExecutorId = userId;
            updateTicket.StatusId = 2;
            context.Update(updateTicket);
            context.SaveChanges();

            return Ok();
        }

        //api/tickets/setStatus
        [HttpPost("setStatus")]
        public async Task<IActionResult> SetStatus([FromBody] JsonObject json)
        {
            long statusId;
            long ticketId;

            if (!(
                json.ContainsKey("statusId") && long.TryParse(json["statusId"]!.GetValue<string>(), out statusId) &&
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId)))
            {
                return BadRequest();
            }

            var updateTicket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (updateTicket == null)
            {
                return NotFound();
            }


            updateTicket.StatusId = statusId;

            context.Update(updateTicket);
            context.SaveChanges();

            return Ok();
        }


        //api/tickets/closeTicket
        [HttpPost("closeTicket")]
        public async Task<IActionResult> CloseTicket([FromBody] JsonObject json)
        {
            long ticketId;
            int finishStatus;
            string commentText;

            if (!(
                json.ContainsKey("ticketId") &&
                json.ContainsKey("finishStatus") &&
                json.ContainsKey("commentText")))
            {
                return BadRequest();
            }

            ticketId = json["ticketId"]!.GetValue<long>();
            finishStatus = json["finishStatus"]!.GetValue<int>();
            commentText = json["commentText"]!.GetValue<string>();

            var updateTicket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (updateTicket == null)
            {
                return NotFound();
            }

            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            Comment newComment = new Comment() { Text = "Пользователь завершил работу над заявкой:\n\n" + commentText, Date = DateTime.Now, TicketId = ticketId, UserId = user.Id, CommentType = CommentType.Official };

            context.Comments.Add(newComment);

            updateTicket.Finished = true;
            updateTicket.FinishStatus = (FinishStatus)finishStatus;
            updateTicket.DeadlineTime = DateTime.Now;
            updateTicket.StatusId = 3;
            context.Update(updateTicket);
            context.SaveChanges();

            return Ok();
        }

        // POST: api/Tickets/reopenTicket
        [HttpPost("reopenTicket")]
        public async Task<IActionResult> ReopenTicket([FromBody] JsonObject json)
        {
            long ticketId;

            if (!(
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId)))
            {
                return BadRequest();
            }

            var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
                return NotFound();

            ticket.StatusId = 5;
            ticket.DeadlineTime = DateTime.Now + new TimeSpan(3, 0, 0);

            await context.SaveChangesAsync();

            return Ok();
        }


        // POST: api/Tickets/SetDeadline
        [HttpPost("setDeadline")]
        public async Task<IActionResult> SetDeadline([FromBody] JsonObject json)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            long ticketId;
            DateTime deadlineTime;

            if (!(
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId) &&
                json.ContainsKey("deadlineTime") && DateTime.TryParse(json["deadlineTime"]!.GetValue<string>(), out deadlineTime)))
            {
                return BadRequest();
            }

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            var updateTicket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (updateTicket == null)
            {
                return NotFound();
            }

            updateTicket.DeadlineTime = deadlineTime;
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("changeSender")]
        public async Task<IActionResult> ChangeSender([FromBody] JsonObject json)
        {
            long ticketId;
            long userId;

            if (!(
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId) &&
                json.ContainsKey("userId") && long.TryParse(json["userId"]!.GetValue<string>(), out userId)))
            {
                return BadRequest();
            }

            Ticket? ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.SenderId = userId;

            await context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(long id)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (!user.AccessGroup.CanDeleteTickets)
                return Forbid();

            var ticket = await context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            context.Tickets.Remove(ticket);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
