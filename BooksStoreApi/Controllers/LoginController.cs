using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BooksStore.Api.RequestModels;
using BooksStore.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BooksStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private List<User> appUsers = new List<User>
            {
            new User { FullName = "Test User", UserName = "user", Password = "1234", UserRole = "User" }
            };

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public IActionResult Login([FromBody]LoginRequestModel login)
        {
            IActionResult response = Unauthorized();
            User user = AuthenticateUser(login.UserName, login.Password);
            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                response = Ok(new
                {
                    token = tokenString,
                    userDetails = user,
                });
            }
            return response;
        }
        User AuthenticateUser(string userName, string password)
        {
            User user = appUsers.SingleOrDefault(x => x.UserName == userName && x.Password == password);
            return user;
        }
        string GenerateJWTToken(User userInfo)
        {
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(2),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
