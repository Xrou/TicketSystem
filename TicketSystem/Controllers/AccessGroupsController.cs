using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using TicketSystem.Models;
using Newtonsoft.Json.Linq;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccessGroupsController : ControllerBase
    {
        private readonly Database context;

        public AccessGroupsController(Database context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<SendAccessGroup>>> GetAccessGroups()
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid();

            return context.AccessGroups.Select(x => x.ToSend()).ToList();
        }

        [HttpPost("UpdateAccessGroups")]
        public async Task<ActionResult> UpdateAccessGroups([FromBody] JsonObject json)
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid();

            JObject jsonObj = JObject.Parse(json.ToString());
            IList<string> groupIds = jsonObj.Properties().Select(p => p.Name).ToList();

            foreach (string groupId in groupIds)
            {
                Dictionary<string, string> dictObj = jsonObj[groupId].ToObject<Dictionary<string, string>>();
                AccessGroup accessGroup = context.AccessGroups.Single(x => x.Id == long.Parse(groupId));

                foreach (var key in dictObj.Keys)
                {
                    switch (key)
                    {
                        case "canSubscribe":
                            accessGroup.CanSubscribe = dictObj[key] == "True";
                            break;
                        case "canSeeHisTickets":
                            accessGroup.CanSeeHisTickets = dictObj[key] == "True";
                            break;
                        case "canSeeCompanyTickets":
                            accessGroup.CanSeeCompanyTickets = dictObj[key] == "True";
                            break;
                        case "canSeeAllTickets":
                            accessGroup.CanSeeAllTickets = dictObj[key] == "True";
                            break;
                        case "canEditTickets":
                            accessGroup.CanEditTickets = dictObj[key] == "True";
                            break;
                        case "canDeleteTickets":
                            accessGroup.CanDeleteTickets = dictObj[key] == "True";
                            break;
                        case "canSeeServiceComments":
                            accessGroup.CanSeeServiceComments = dictObj[key] == "True";
                            break;
                        case "canRegisterUsers":
                            accessGroup.CanRegisterUsers = dictObj[key] == "True";
                            break;
                        case "canSelectTopic":
                            accessGroup.CanSelectTopic = dictObj[key] == "True";
                            break;
                        case "canEditUsers":
                            accessGroup.CanEditUsers = dictObj[key] == "True";
                            break;
                        case "canSelectUrgency":
                            accessGroup.CanSelectUrgency = dictObj[key] == "True";
                            break;
                        case "canTakeTickets":
                            accessGroup.CanTakeTickets = dictObj[key] == "True";
                            break;
                        case "canAssignTickets":
                            accessGroup.CanAssignTickets = dictObj[key] == "True";
                            break;
                        case "canFinishTickets":
                            accessGroup.CanFinishTickets = dictObj[key] == "True";
                            break;
                        case "canMoveTickets":
                            accessGroup.CanMoveTickets = dictObj[key] == "True";
                            break;
                    }
                }
            }

            context.SaveChanges();

            return Ok();
        }
    }
}
