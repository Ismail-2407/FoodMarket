// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using FoodMarket.Models;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using System.Collections.Generic;
//
// namespace FoodMarket.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class AccountController : ControllerBase
//     {
//         private readonly UserManager<User> _userManager;
//         private readonly IConfiguration _configuration;
//
//         public AccountController(UserManager<User> userManager, IConfiguration configuration)
//         {
//             _userManager = userManager;
//             _configuration = configuration;
//         }
//
//         [HttpPost("login")]
//         public async Task<IActionResult> Login([FromBody] LoginModel model)
//         {
//             var user = await _userManager.FindByEmailAsync(model.Email);
//             // if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
//             //     return Unauthorized("Неверный email или пароль");
//             
//             await _userManager.AddToRoleAsync(user, "User");
//             var roles = await _userManager.GetRolesAsync(user);
//             var role = roles.FirstOrDefault() ?? "User";
//             
//
//             var claims = new List<Claim>
//             {
//                 new Claim(ClaimTypes.NameIdentifier, user.Id),
//                 new Claim(ClaimTypes.Email, user.Email),
//                 new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
//                 new Claim(ClaimTypes.Role, role)
//             };
//
//             //foreach (var role in roles)
//             //{
//                // claims.Add(new Claim(ClaimTypes.Role, role));
//             //}
//
//             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//
//             var token = new JwtSecurityToken(
//                 issuer: _configuration["Jwt:Issuer"],
//                 audience: _configuration["Jwt:Audience"],
//                 claims: claims,
//                 expires: DateTime.Now.AddMinutes(60),
//                 signingCredentials: creds
//             );
//
//             var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
//
//             return Ok(new { token = tokenString });
//         }
//     }
//
//     public class LoginModel
//     {
//         public string Email { get; set; }
//         public string Password { get; set; }
//     }
// }
