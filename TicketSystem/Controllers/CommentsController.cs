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
    public class CommentsController : ControllerBase
    {
        private readonly Database context;

        public CommentsController(Database context)
        {
            this.context = context;
        }

        // GET: api/Comments/5      5 - ticketId
        [HttpGet("{ticketId}")]
        public async Task<ActionResult<string>> GetCommentsAtMessage(long ticketId)
        {
            if (context.Comments == null)
            {
                return NotFound();
            }

            List<SendComment> comments = new List<SendComment>();

            foreach (var comment in context.Comments.Where(c => c.TicketId == ticketId).OrderBy(c => c.Date))
            {
                comments.Add(comment.ToSend());
            }

            return JsonConvert.SerializeObject(comments);
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IResult> PostComment(PostComment comment)
        {
            Claim? idClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            long userId;

            if (idClaim == null || !long.TryParse(idClaim.Value, out userId))
                return Results.Problem("Incorrectly authenticated user");

            long commentId = context.Comments.Max(x => x.Id) + 1;
            DateTime commentDate = DateTime.Now;

            if (context.Comments == null)
            {
                return Results.Problem("Entity set 'TicketContext.CommentItems' is null.");
            }

            Comment newComment = new Comment() { Id = commentId, Text = comment.Text, Date = commentDate, TicketId = comment.TicketId, UserId = userId };

            context.Comments.Add(newComment);
            await context.SaveChangesAsync();

            return Results.Created(new Uri("https://localhost:7177/api/comments"), commentId);
        }
    }
}
