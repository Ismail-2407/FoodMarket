using FoodMarket.Models;
using FoodMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace FoodMarket.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public ResetPasswordController(UserManager<User> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Ok(); 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{dto.ClientUrl}/reset-password?email={WebUtility.UrlEncode(dto.Email)}&token={WebUtility.UrlEncode(token)}";

            var body = $"<p>Для сброса пароля перейдите по ссылке: <a href='{callbackUrl}'>Сбросить пароль</a></p>";
            await _emailSender.SendEmailAsync(dto.Email, "Сброс пароля", body);

            return Ok();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Пользователь не найден");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Пароль успешно изменён");
        }
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public string ClientUrl { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
