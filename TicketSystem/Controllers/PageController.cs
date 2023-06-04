using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace TicketSystem.Controllers
{
    [Route("/")]
    public class PageController : Controller
    {
        [HttpGet]
        public IResult Index()
        {
            return ReadFile("Content/index.html");
        }

        [HttpGet("indexStyles.css")]
        public IResult IndexStyles()
        {
            return ReadFile("Content/indexStyles.css");
        }

        [HttpGet("auth")]
        public IResult Auth()
        {
            return ReadFile("Content/auth.html");
        }

        [HttpGet("authStyles.css")]
        public IResult AuthStyles()
        {
            return ReadFile("Content/authStyles.css");
        }

        [HttpGet("register")]
        public IResult Register()
        {
            return ReadFile("Content/register.html");
        }

        [HttpGet("registerStyles.css")]
        public IResult RegisterStyles()
        {
            return ReadFile("Content/registerStyles.css");
        }

        [HttpGet("createTicket")]
        public IResult CreateTicket()
        {
            return ReadFile("Content/createTicket.html");
        }

        [HttpGet("createTicketStyles.css")]
        public IResult CreateTicketStyles()
        {
            return ReadFile("Content/createTicketStyles.css");
        }

        [HttpGet("ticket/{id}")]
        public IResult GetTicket()
        {
            return ReadFile("Content/ticket.html");
        }

        [HttpGet("ticketStyles.css")]
        public IResult GetTicketStyles()
        {
            return ReadFile("Content/ticketStyles.css");
        }

        private IResult ReadFile(string filePath)
        {
            string fileFormat = Path.GetExtension(filePath)[1..];

            string file = System.IO.File.ReadAllText(filePath);
            return Results.Content(file, $"text/{fileFormat}");
        }
    }
}
