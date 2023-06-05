using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            if (context.Tickets == null)
            {
                return NotFound();
            }

            List<SendTicket> tickets = new List<SendTicket>();

            foreach (var ticket in context.Tickets)
            {
                tickets.Add(ticket.ToSend());
            }

            return JsonConvert.SerializeObject(tickets);
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendTicket>> GetTicket(long id)
        {
            if (context.Tickets == null)
            {
                return NotFound();
            }
            var ticket = await context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket.ToSend();
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(long id, SendTicket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IResult> PostTicket(PostTicket ticket)
        {
            Claim? idClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            long userId;

            if (idClaim == null || !long.TryParse(idClaim.Value, out userId))
                return Results.Problem("Incorrectly authenticated user");

            long ticketId;

            if (context.Tickets.Count() != 0)
                ticketId = context.Tickets.Max(x => x.Id) + 1;
            else
                ticketId = 1;
            
            DateTime ticketDate = DateTime.Now;

            if (context.Tickets == null)
            {
                return Results.Problem("Entity set 'TicketContext.TicketItems'  is null.");
            }

            Ticket newTicket = new Ticket() { Id = ticketId, Date = ticketDate, Text = ticket.Text, UserId = userId };

            context.Tickets.Add(newTicket);
            await context.SaveChangesAsync();

            return Results.Created(new Uri("https://localhost:7177/api/tickets"), ticketId);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(long id)
        {
            if (context.Tickets == null)
            {
                return NotFound();
            }
            var ticket = await context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            context.Tickets.Remove(ticket);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(long id)
        {
            return (context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
