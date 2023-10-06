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
        public async Task<ActionResult<IEnumerable<SendUserGroupShort>>> GetUserGroups()
        {
            return context.UserGroups.Select(ug => ug.ToSendShort()).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SendUserGroup>> GetUserGroup(long id)
        {
            var group = context.UserGroups.FirstOrDefault(ug => ug.Id == id);

            if (group == null)
                return NotFound();

            return group.ToSend();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserGroup([FromBody] JsonObject json)
        {
            if (!(json.ContainsKey("name")))
                return BadRequest();

            UserGroup userGroup = new UserGroup() { Name = json["name"]!.GetValue<string>() };

            context.UserGroups.Add(userGroup);

            context.SaveChanges();
            context.UserGroups.Load();

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> EditUserGroup(long id, [FromBody] JsonObject json)
        {
            UserGroup? userGroup = context.UserGroups.FirstOrDefault(u => u.Id == id);

            if (userGroup == null)
                return BadRequest();

            if (json.ContainsKey("name"))
                userGroup.Name = json["name"]!.GetValue<string>();

            context.SaveChanges();
            context.UserGroups.Load();

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

        [HttpPost("UpdateUserGroups")]
        public async Task<ActionResult> UpdateUserGroups([FromBody] JsonObject json)
        {
            foreach (var user in json)
            {
                long userId = long.Parse(user.Key);
                var groups = user.Value.AsObject();

                User? userObj = context.Users.FirstOrDefault(u => u.Id == userId);

                if (userObj == null)
                    return BadRequest();

                foreach (var group in groups)
                {
                    long groupId = long.Parse(group.Key);
                    bool isNeeded = group.Value.GetValue<bool>();

                    if (isNeeded && !userObj.UserGroups.Any(g => g.Id == groupId))
                    {
                        UserGroup? userGroup = context.UserGroups.FirstOrDefault(g => g.Id == groupId);

                        if (userGroup == null)
                            return BadRequest();

                        userObj.UserGroups.Add(userGroup);
                    }
                    if (!isNeeded && userObj.UserGroups.Any(g => g.Id == groupId))
                    {
                        UserGroup userGroup = context.UserGroups.First(g => g.Id == groupId);

                        userObj.UserGroups.Remove(userGroup);
                    }
                }
            }

            context.SaveChanges();

            return Ok();
        }

    }
}
