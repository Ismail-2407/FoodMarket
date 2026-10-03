// using FoodMarket.Models;
// using FoodMarket.Services.Interfaces;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
//
// namespace FoodMarket.Controllers
// {
//     [Route("api/account")]
//     [ApiController]
//     public class AccountController : ControllerBase
//     {
//         private readonly UserManager<User> _userManager;
//         private readonly IEmailSender _emailSender;
//
//         public AccountController(UserManager<User> userManager, IEmailSender emailSender)
//         {
//             _userManager = userManager;
//             _emailSender = emailSender;
//         }
//
//         [HttpPost("forgot-password")]
//         public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
//         {
//             var user = await _userManager.FindByEmailAsync(dto.Email);
//             if (user == null)
//                 return Ok(new { message = "Если email существует, письмо отправлено" });
//
//             var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//             var encodedToken = Uri.EscapeDataString(token);
//             var resetLink = $"http://localhost:5173/reset-password?email={dto.Email}&token={encodedToken}";
//
//             await _emailSender.SendEmailAsync(dto.Email, "Сброс пароля",
//                 $"<p>Для сброса пароля перейдите по ссылке:</p><a href='{resetLink}'>{resetLink}</a>");
//
//             return Ok(new { message = "Письмо отправлено" });
//         }
//
//         [HttpPost("reset-password")]
//         public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
//         {
//             var user = await _userManager.FindByEmailAsync(dto.Email);
//             if (user == null)
//                 return BadRequest("Пользователь не найден");
//
//             var decodedToken = Uri.UnescapeDataString(dto.Token);
//             var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
//
//             if (!result.Succeeded)
//             {
//                 var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//                 Console.WriteLine($"Reset password errors: {errors}");
//                 return BadRequest(errors);
//             }
//
//             return Ok(new { message = "Пароль успешно изменён" });
//         }
//     }
//
//     public class ForgotPasswordDto
//     {
//         public string Email { get; set; } = "";
//     }
//
//     public class ResetPasswordDto
//     {
//         public string Email { get; set; } = "";
//         public string Token { get; set; } = "";
//         public string NewPassword { get; set; } = "";
//     }
// }