using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<string>> GetTicketItems()
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
            {
                return Problem("No user instance", statusCode: 500);
            }

            List<SendTicket> tickets = new List<SendTicket>();

            foreach (var ticket in context.Tickets.OrderByDescending(t => t.Id))
            {
                if (InternalActions.CanUserAccessTicket(user, ticket))
                    tickets.Add(ticket.ToSend());
            }

            return JsonConvert.SerializeObject(tickets);
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

        protected internal static async Task<Ticket> CreateTicket(Database context, PostTicket ticket, long userId, bool registration = false)
        {
            DateTime ticketDate = DateTime.Now;

            TicketType ticketType = TicketType.Standard;

            if (registration)
                ticketType = TicketType.Registration;

            Ticket newTicket = new Ticket() { Type = ticketType, Date = ticketDate, Text = ticket.Text, DeadlineTime = ticketDate + new TimeSpan(3, 0, 0), UserId = userId };

            context.Tickets.Add(newTicket);
            await context.SaveChangesAsync();

            return newTicket;
        }

        [HttpPost]
        public async Task<IActionResult> PostTicket(PostTicket ticket)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            Ticket newTicket = await CreateTicket(context, ticket, user.Id);

            return Created(new Uri("https://localhost:7177/api/tickets"), newTicket.Id);
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
                json.ContainsKey("ticketId") && long.TryParse(json["ticketId"]!.GetValue<string>(), out ticketId) &&
                json.ContainsKey("finishStatus") && int.TryParse(json["finishStatus"]!.GetValue<string>(), out finishStatus) &&
                json.ContainsKey("commentText")))
            {
                return BadRequest();
            }

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
            context.Update(updateTicket);
            context.SaveChanges();

            return Ok();
        }

        // POST: api/Tickets/SetDeadline
        [HttpPost("{id}")]
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
