using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodMarket.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodMarket.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id),
                new Claim("email", user.Email),
                new Claim("name", user.UserName ?? user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: identity.Claims, // ✅ используем identity.Claims
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"📏 Token length: {jwt.Length}");
            Console.WriteLine($"🆔 Token: {jwt}");

            return jwt;
        }


        public string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }
}
