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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Configuration;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities.Encoders;
using Telegram.Bot.Extensions.LoginWidget;
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
            try
            {
                string passwordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(userToAuth.Password)));

                User? user = context.Users.FirstOrDefault(u => u.Name == userToAuth.Login && u.PasswordHash == passwordHash);

                if (user is null)  // !user.CanLogin - если подтвержденный аккаунт
                    return Results.Unauthorized();

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userToAuth.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(12)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = userToAuth.Login,
                    id = user.Id
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
        public async Task<IActionResult> Register([FromBody] JsonObject json)
        {
            try
            {
                string login;
                string password;
                string verificationCode;

                string fullName;
                string phoneNumber;
                string email;
                string company;

                if (!(
                    json.ContainsKey("login") &&
                    json.ContainsKey("password") &&
                    json.ContainsKey("verificationCode") &&
                    json.ContainsKey("fullName") &&
                    json.ContainsKey("phoneNumber") &&
                    json.ContainsKey("email") &&
                    json.ContainsKey("company")
                    ))
                {
                    return BadRequest();
                }

                login = json["login"]!.GetValue<string>();
                password = json["password"]!.GetValue<string>();
                verificationCode = json["verificationCode"]!.GetValue<string>();

                fullName = json["fullName"]!.GetValue<string>();
                phoneNumber = json["phoneNumber"]!.GetValue<string>();
                email = json["email"]!.GetValue<string>();
                company = json["company"]!.GetValue<string>();

                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync($"http://localhost:8888/code/?code={verificationCode}");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return Forbid();
                }

                string telegramIdString = await response.Content.ReadAsStringAsync();

                long telegramId = long.Parse(telegramIdString);
                string passwordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(password)));

                User newUser = new User() { Name = login, CompanyId = 1, PasswordHash = passwordHash, AccessGroupId = 1, FullName = fullName, PhoneNumber = phoneNumber, Email = email, Telegram = telegramId };
                context.Users.Add(newUser);

                await context.SaveChangesAsync();

                string registrationText = "Новый пользователь регистрируется в системе.\n" +
                    $"Логин: {login}\n" +
                    $"Пароль: {password}\n" +
                    $"ФИО: {fullName}\n" +
                    $"Компания: {company}\n" +
                    $"Номер телефона: {phoneNumber}\n" +
                    $"Электронная почта: {email}\n";

                await TicketsController.CreateTicket(context, HttpContext, new PostTicket(newUser.Id, registrationText, 3, 1, new Models.File[] { }), newUser.Id, true);

                return Created(new Uri("https://localhost:7177/api/users/register"), newUser.Id);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Source}: {ex.Message}");
                return Problem();
            }
        }

        [HttpPost("loginTelegram")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTelegram([FromBody] JsonObject json)
        {
            var authData = new SortedDictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString()));

            LoginWidget loginWidget = new LoginWidget("5397081584:AAHAYI5fiEPRcL0LQeTuASzU78-EkaduqUg");
            var auth = loginWidget.CheckAuthorization(authData);
            if (auth == Authorization.Valid)
            {
                User? user = context.Users.FirstOrDefault(u => u.Telegram == Convert.ToInt64(authData["id"]));

                if (user == null)
                {
                    return NotFound();
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(12)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = user.Name,
                    id = user.Id
                };

                return Ok(response);
            }

            return Forbid();
        }

        [HttpPost("confirmRegistration")]
        public async Task<IActionResult> ConfirmRegistration([FromBody] JsonObject json)
        {
            try
            {
                if (!(
                    json.ContainsKey("ticketId") &&
                    json.ContainsKey("fullName") &&
                    json.ContainsKey("phone") &&
                    json.ContainsKey("email") &&
                    json.ContainsKey("companyId")
                    ))
                {
                    return BadRequest();
                }

                long ticketId = Convert.ToInt64(json["ticketId"]!.GetValue<string>());
                string fullName = json["fullName"]!.GetValue<string>();
                string phone = json["phone"]!.GetValue<string>();
                string email = json["email"]!.GetValue<string>();
                long companyId = json["companyId"]!.GetValue<long>();

                Ticket? ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);
                long? userId = ticket?.UserId;
                long? executorId = ticket?.ExecutorId;

                if (userId == null)
                {
                    return Problem();
                }

                if (executorId == null)
                {
                    return BadRequest("executor not assigned");
                }

                User user = context.Users.First(u => u.Id == userId);
                user.CanLogin = true;
                user.FullName = fullName;
                user.PhoneNumber = phone;
                user.Email = email;
                user.CompanyId = companyId;

                await context.SaveChangesAsync();

                Comment newComment = new Comment() { Text = "Новый пользователь зарегистрирован", Date = DateTime.Now, TicketId = ticketId, UserId = executorId.GetValueOrDefault(), CommentType = CommentType.Official };

                context.Comments.Add(newComment);

                ticket.Finished = true;
                ticket.FinishStatus = FinishStatus.Incident;
                ticket.DeadlineTime = DateTime.Now;
                context.Update(ticket);
                context.SaveChanges();
                /*
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync($"http://localhost:8888/registrationVerified/?telegram={user.Telegram}");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return Problem("Telegram error");
                }
                */
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Source}: {ex.Message} {ex.StackTrace}");
                return Problem();
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendUser>>> GetUsers(int? page, string? login = null, 
            string? phone = null, string? pcName = null, long? companyId = null)
        {
            if (page <= 0)
            {
                return BadRequest("Page must be positive number");
            }

            IQueryable<User> users = context.Users;

            if (page == null)
                page = 1;

            if (login != null)
                users = users.Where(x => x.Name == login);

            if (phone != null)
                users = users.Where(x => x.PhoneNumber == phone);

            //if (pcName != null)
            //    users.Where(x => x.PCName == pcName);

            if (companyId != null)
                users = users.Where(x => x.Company.Id == companyId);

            return users.Select(x => x.ToSend()).Page((int)page, 5).ToList();
        }

        // POST: api/Users/getFilteredUsers
        [HttpPost("getFilteredUsers")]
        public async Task<ActionResult<IEnumerable<SendUser>>> GetFilteredUsers([FromBody] JsonObject json)
        {
            IEnumerable<User> users = context.Users.ToList();

            if (json.ContainsKey("id") && long.TryParse(json["id"]!.GetValue<string>(), out long id))
            {
                users = users.Where(x => x.Id == id);
            }
            else
            {
                if (json.ContainsKey("name"))
                {
                    string nameValue = json["name"]!.GetValue<string>().ToLower();

                    users = users.Where(x =>
                    {
                        if (x.FullName == null)
                            return false;

                        if (x.FullName!.ToLower().Contains(nameValue))
                            return true;

                        return false;
                    });
                }

                if (json.ContainsKey("admins"))
                {
                    if (json["admins"]!.GetValue<bool>() == true)
                        users = users.Where(x => x.AccessGroupId >= 3);
                }

                if (json.ContainsKey("company"))
                {
                    string companyValue = json["company"]!.GetValue<string>().ToLower();

                    users = users.Where(x =>
                    {
                        if (x.Company.Name.ToLower().Contains(companyValue))
                            return true;

                        return false;
                    });
                }
            }

            return users.Select(x => x.ToSend()).ToList();
        }

        // POST: api/Users/getFilteredUsers
        [HttpPost("getExecutors")]
        public async Task<ActionResult<IEnumerable<SendUser>>> GetExecutors([FromBody] JsonObject json)
        {
            IEnumerable<User> users = context.Users.Where(x => x.AccessGroupId > 2).ToList();

            if (json.ContainsKey("id") && long.TryParse(json["id"]!.GetValue<string>(), out long id))
            {
                users = users.Where(x => x.Id == id);
            }
            else
            {
                if (json.ContainsKey("name"))
                {
                    string nameValue = json["name"]!.GetValue<string>().ToLower();

                    users = users.Where(x =>
                    {
                        if (x.FullName == null)
                            return false;

                        if (x.FullName!.ToLower().Contains(nameValue))
                            return true;

                        return false;
                    });
                }
            }

            return users.Select(x => x.ToSend()).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendUser>> GetUser(long id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user.ToSend();
        }

        // POST: api/Users/5
        [HttpPost("{id}")]
        public async Task<ActionResult<SendUser>> UpdateUser(long id, [FromBody] JsonObject json)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (json.ContainsKey("name"))
                user.Name = json["name"]!.GetValue<string>();

            if (json.ContainsKey("fullName"))
                user.FullName = json["fullName"]!.GetValue<string>();

            if (json.ContainsKey("companyId"))
                user.CompanyId = json["companyId"]!.GetValue<int>();

            if (json.ContainsKey("phoneNumber"))
                user.PhoneNumber = json["phoneNumber"]!.GetValue<string>();

            if (json.ContainsKey("email"))
                user.Email = json["email"]!.GetValue<string>();

            if (json.ContainsKey("telegram"))
                user.Telegram = json["telegram"]!.GetValue<long>();

            context.SaveChanges();

            return Ok();
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

        [HttpGet("canRegisterUsers")]
        public async Task<ActionResult<string>> CanRegisterUsers()
        {
            User? user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return NotFound();

            return JsonConvert.SerializeObject(user.AccessGroup.CanRegisterUsers);
        }

        [HttpGet("hasAdminRights")]
        public async Task<IActionResult> HasAdminRights()
        {
            var user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return NotFound();

            if (user.AccessGroup.Id == 5)
            {
                return Ok();
            }

            return Forbid();
        }

        [HttpGet("getRights")]
        public async Task<ActionResult<string>> GetRights()
        {
            var user = InternalActions.SelectUserFromContext(HttpContext, context);

            if (user == null)
                return NotFound();

            return JsonConvert.SerializeObject(user.AccessGroup.ToSend());
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

        [HttpGet("GetUsersGroups")]
        public async Task<ActionResult<ICollection<SendUserGroups>>> GetUsersGroups()
        {
            if (InternalActions.SelectUserFromContext(HttpContext, context)?.AccessGroupId != 5)
                return Forbid();

            return context.Users.Select(user => user.ToSendUserGroups()).ToList();
        }
    }
}
