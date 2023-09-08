using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json.Nodes;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserGroupsController : Controller
    {
        private readonly Database context;

        public UserGroupsController(Database context)
        {
            this.context = context;
            context.UserGroups.Include(ug => ug.Users).Load();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendUserGroup>>> GetUserGroups()
        {
            return context.UserGroups.Select(ug => ug.ToSend()).ToList();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserGroup([FromBody]JsonObject json)
        {
            if (!(json.ContainsKey("name")))
                return BadRequest();

            UserGroup userGroup = new UserGroup() { Name = json["name"]!.GetValue<string>() };

            context.UserGroups.Add(userGroup);

            context.SaveChanges();

            return Ok();
        }

        [HttpPost("AddUserInGroup")]
        public async Task<ActionResult> AddUserInGroup([FromBody] JsonObject json)
        {
            if (!(
                json.ContainsKey("userId") &&
                json.ContainsKey("groupId")))
                return BadRequest();

            var user = context.Users.FirstOrDefault(u => u.Id == json["userId"]!.GetValue<long>());

            if (user == null) 
                return BadRequest();

            var userGroup = context.UserGroups.FirstOrDefault(ug => ug.Id == json["groupId"]!.GetValue<long>());

            if (userGroup == null)
                return BadRequest();

            user.UserGroups.Add(userGroup);
            context.SaveChanges();

            context.UserGroups.Load();

            return Ok();
        }
    }
}
