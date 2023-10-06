using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/statuses")]
    [ApiController]
    [Authorize]
    public class StatusController : Controller
    {
        private readonly Database context;

        public StatusController(Database context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetStatusItems()
        {
            List<SendStatus> statuses = context.Statuses.Select(x => x.ToSend()).ToList();

            return JsonConvert.SerializeObject(statuses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromBody] JsonObject json)
        {
            if (!(json.ContainsKey("name")))
            {
                return BadRequest();
            }

            string name = json["name"]!.GetValue<string>();

            Status newStatus = new Status() { Name = name };

            context.Statuses.Add(newStatus);
            context.SaveChanges();

            return Ok();
        }

    }
}
