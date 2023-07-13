using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly Database context;

        public UsersController(Database context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IResult> Login(AuthorizeUser userToAuth)
        {
            if (userToAuth.Login.ToLower() == "bot")
                return Results.Forbid();

            try
            {
                string passwordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(userToAuth.Password)));

                User? user = context.Users.FirstOrDefault(u => u.Name == userToAuth.Login && u.PasswordHash == passwordHash);
                if (user is null) return Results.Unauthorized();

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userToAuth.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(12)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = userToAuth.Login
                };
                return Results.Json(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.Unauthorized();
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IResult> Register(AuthorizeUser userToRegister)
        {
            try
            {
                string passwordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(userToRegister.Password)));

                User newUser = new User() { Name = userToRegister.Login, CompanyId = 1, PasswordHash = passwordHash, AccessGroupId = 1 };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                return Results.Created(new Uri("https://localhost:7177/api/users/register"), newUser.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.Unauthorized();
            }
        }

        [HttpPost("temporarylogin")]
        [AllowAnonymous]
        public async Task<IResult> TemporaryLogin(TemporaryAuthorizeUser userToAuth)
        {
            TemporaryAuthorizeUser? user = Storage.TemporaryAuthorizeUsers.FirstOrDefault(u => u.TelegramId == userToAuth.TelegramId && u.Code == userToAuth.Code);

            if (user is null) return Results.Unauthorized();

            Storage.TemporaryAuthorizeUsers.Remove(user);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userToAuth.TelegramId.ToString()) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = userToAuth.TelegramId
            };

            return Results.Json(response);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendUser>>> GetUsers()
        {
            if (context.Users == null)
            {
                return NotFound();
            }

            List<SendUser> sendUsers = new List<SendUser>();

            foreach (var user in context.Users)
            {
                sendUsers.Add(user.ToSend());
            }

            return sendUsers;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendUser>> GetUser(long id)
        {
            if (context.Users == null)
            {
                return NotFound();
            }
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user.ToSend();
        }

        // GET: api/Users/me
        [HttpGet("me")]
        public async Task<ActionResult<SendUser>> GetMe()
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
            {
                return NotFound();
            }

            return user.ToSend();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            context.Entry(user).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (context.Users == null)
            {
                return Problem("Entity set 'UserContext.TodoItems'  is null.");
            }

            if (await context.Users.FindAsync(user.Id) != null)
            {
                return Problem("Entity already in database");
            }

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (context.Users == null)
            {
                return NotFound();
            }
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return (context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
