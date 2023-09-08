using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
    public class CompaniesController : ControllerBase
    {
        private readonly Database context;

        public CompaniesController(Database context)
        {
            this.context = context;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendCompany>>> GetCompaniesItems()
        {
            List<SendCompany> companies = context.Companies.Where(x => x.Id != 1).Select(x => x.ToSend()).ToList();

            return companies;
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendCompany>> GetCompany(long id)
        {
            SendCompany? companies = context.Companies.FirstOrDefault(x => x.Id == id)?.ToSend();

            if (companies == null)
            {
                return NotFound();
            }

            return companies;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] JsonObject json)
        {
            if (!(
                json.ContainsKey("name") &&
                json.ContainsKey("shortName")
                ))
            {
                return BadRequest();
            }

            string name = json["name"]!.GetValue<string>();
            string shortName = json["shortName"]!.GetValue<string>();

            Company company = new Company() { Name = name, ShortName = shortName };

            context.Companies.Add(company);

            context.SaveChanges();

            return Ok(company.Id);
        }

        // POST: api/Companies/5
        [HttpPost("{id}")]
        public async Task<IActionResult> EditCompany(long id, [FromBody] JsonObject json)
        {
            Company? company = context.Companies.FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            if (json.ContainsKey("name"))
                company.Name = json["name"]!.GetValue<string>();

            if (json.ContainsKey("shortName"))
                company.ShortName = json["shortName"]!.GetValue<string>();

            context.SaveChanges();
            
            return Ok();
        }
    }
}
