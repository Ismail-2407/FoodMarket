using FoodMarket.Models;
using FoodMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FoodMarket.Controllers;

[ApiController]
[Route("api/account")]
public class ResetPasswordController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;
    
    private static readonly Dictionary<string, string> _resetTokens = new();

    public ResetPasswordController(IUserService userService, IEmailSender emailSender)
    {
        _userService = userService;
        _emailSender = emailSender;
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user == null)
            return Ok(); 
        
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _resetTokens[user.Email] = token;

        var callbackUrl = $"{dto.ClientUrl}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(token)}";

        var body = $"<p>Для сброса пароля перейдите по ссылке: <a href='{callbackUrl}'>Сбросить пароль</a></p>";
        await _emailSender.SendEmailAsync(user.Email, "Сброс пароля", body);

        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Пользователь не найден");

        if (!_resetTokens.TryGetValue(dto.Email, out var storedToken) || storedToken != dto.Token)
            return BadRequest("Неверный или просроченный токен");
        
        _resetTokens.Remove(dto.Email);

        var updated = await _userService.UpdatePasswordAsync(user.Id, dto.NewPassword);
        if (!updated)
            return BadRequest("Не удалось обновить пароль");

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
