using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly Database context;

        public ServiceController(Database context)
        {
            this.context = context;
        }

        [HttpPost("createtemporaryauthorization")]
        public async Task CreateTemporaryAuthorization(TemporaryAuthorizeUser user)
        {
            Storage.TemporaryAuthorizeUsers.Add(new TemporaryAuthorizeUser(user.TelegramId, user.Code));
        }
    }
}
