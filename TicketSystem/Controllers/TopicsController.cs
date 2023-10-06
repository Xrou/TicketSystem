using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TopicsController : Controller
    {
        private readonly Database context;

        public TopicsController(Database context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetTopicItems()
        {
            List<SendTopic> topics = context.Topics.Select(x => x.ToSend()).ToList();

            return JsonConvert.SerializeObject(topics);
        }

        [HttpPost]
        public async Task<IActionResult> PostTopic([FromBody] JsonObject json)
        {
            if (!(json.ContainsKey("name")))
            {
                return BadRequest();
            }

            string name = json["name"]!.GetValue<string>();

            Topic newTopic = new Topic() { Name = name };

            context.Topics.Add(newTopic);
            context.SaveChanges();

            return Ok();
        }

    }
}
