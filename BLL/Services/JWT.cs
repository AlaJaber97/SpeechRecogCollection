using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public static class JWT
    {
        public static async Task<string> GenerateJwtToken(string email, BLL.Sql.Models.User user, IConfiguration _configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("Gender",user.Gender.ToString()),
                new Claim("Age",user.Age.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static BLL.Sql.Models.User GetUser(string Token)
        {
            if (string.IsNullOrEmpty(Token)) return null;
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(Token);
                var tokenS = handler.ReadToken(Token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                return new BLL.Sql.Models.User
                {
                    Id = tokenS.Claims.First(claim => claim.Type == "jti").Value,
                    Email = tokenS.Claims.First(claim => claim.Type == "sub").Value,
                    Gender = (BLL.Enum.Gender)System.Enum.Parse(typeof(BLL.Enum.Gender), tokenS.Claims.First(claim => claim.Type == "Gender").Value),
                    Age = int.Parse(tokenS.Claims.First(claim => claim.Type == "Age").Value),
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
