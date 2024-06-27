using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class CommentsController : ControllerBase
    {
        private readonly Database context;

        public CommentsController(Database context)
        {
            this.context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<List<SendComment>>> GetCommentsAtMessage(string ticketId, string commentType)
        {
            int commentTypeInt = int.Parse(commentType);
            long ticketIdLong = long.Parse(ticketId);

            List<SendComment> comments = new List<SendComment>();
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (!InternalActions.CanUserAccessTicket(user, context.Tickets.First(t => t.Id == ticketIdLong)))
                return Forbid();

            if (commentTypeInt == 3 && !user.AccessGroup.CanSeeServiceComments)
                return Forbid();

            foreach (var comment in context.Comments.Where(c => c.TicketId == ticketIdLong).OrderByDescending(c => c.Date))
            {
                if ((int)comment.CommentType == commentTypeInt)
                    comments.Add(comment.ToSend());
            }

            return comments;
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostComment(PostComment comment)
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return Problem("No user instance", statusCode: 500);

            if (!InternalActions.CanUserAccessTicket(user, context.Tickets.First(t => t.Id == comment.TicketId)))
                return Forbid();

            if (comment.type == (int)CommentType.Service && !user.AccessGroup.CanSeeServiceComments)
                return Forbid();

            long userId;

            DateTime commentDate = DateTime.Now;

            Comment newComment = new Comment() { Text = comment.Text, Date = commentDate, TicketId = comment.TicketId, UserId = user.Id, CommentType = (CommentType)comment.type };

            context.Comments.Add(newComment);
            await context.SaveChangesAsync();

            if (comment.Files != null)
            {
                foreach (var file in comment.Files)
                {
                    newComment.Files.Add(file);
                }
            }

            await context.SaveChangesAsync();

            try
            {
                var oldUserId = context.Merge.FirstOrDefault(u => u.NewId == user.Id && u.Entity == "user")?.OldId;
                var oldTicketId = context.Merge.FirstOrDefault(u => u.NewId == comment.TicketId && u.Entity == "ticket")?.OldId;

                Task task = new Task(async () =>
                {
                    External.SUZApi api = new External.SUZApi();
                    if (oldUserId is not null && oldTicketId is not null)
                    {
                        await api.SendCommentAsync(oldUserId.ToString(), user.Telegram.ToString(), oldTicketId.ToString(), comment.Text);
                    }
                });

                task.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Created(new Uri("https://localhost:7177/api/comments"), newComment.Id);
        }
    }
}
