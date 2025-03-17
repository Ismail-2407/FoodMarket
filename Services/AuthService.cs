using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodMarket.Data;
using FoodMarket.DTOs;
using FoodMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodMarket.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string?> Register(RegisterDto dto)
        {
            var user = new User { FullName = dto.FullName, Email = dto.Email, UserName = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            return result.Succeeded ? GenerateJwtToken(user) : null;
        }

        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Id) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}