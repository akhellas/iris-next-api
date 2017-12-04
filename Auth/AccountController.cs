using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Iris.Api.Entities;
using MongoDB.Driver;

namespace Iris.Api
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private DataContext _db;
        private readonly IConfiguration _config;

        public AccountController(DataContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbUser = await _db.Users.Find(t => t.Username == model.Username).FirstOrDefaultAsync();

                if (dbUser != null && PasswordHasher.VerifyHashedPassword(dbUser.Password, model.Password))
                {
                    var claims = new List<Claim>();

                    claims.Add(new Claim("Id", dbUser.Id));
                    claims.Add(new Claim("Username", dbUser.Username));

                    foreach (var item in dbUser.Roles)
                    {
                        claims.Add(new Claim("role", item));
                    }

                    foreach (var item in dbUser.Claims)
                    {
                        claims.Add(new Claim(item.Type, item.Value));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                      _config["Tokens:Issuer"],
                      claims,
                      expires: DateTime.Now.AddDays(30),
                      signingCredentials: creds);

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

                }
            }

            return BadRequest("Could not create token");
        }
    }
}