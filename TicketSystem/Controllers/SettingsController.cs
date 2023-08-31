using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Configuration;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly Database context;

        public SettingsController(Database context)
        {
            this.context = context;
        }

        [HttpGet("GetUsersAccessGroup")]
        public async Task<ActionResult<ICollection<SendUserAccessGroup>>> GetUsersAccessGroups()
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid(); 
            return context.Users.Select(user => user.ToSendUserAccessGroup()).ToList();
        }

        [HttpGet("GetUserAccessGroup")]
        public async Task<ActionResult<SendUserAccessGroup>> GetUserAccessGroup(long id)
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid(); 
            var user = context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null)
                return NotFound();

            return user.ToSendUserAccessGroup();
        }

        [HttpPost("UpdateUsersAccessGroups")]
        public async Task<IActionResult> UpdateUsersAccessGroups([FromBody] JsonObject json)
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid();

            foreach (var item in json)
            {
                long id = long.Parse(item.Key);
                var user = context.Users.FirstOrDefault(user => user.Id == id);

                if (user == null)
                    continue;

                user.AccessGroupId = item.Value.GetValue<int>();
                context.Users.Update(user);
            }

            context.SaveChanges();

            return Ok();
        }
    }
}
