using FoodMarket.API.DTOs;
using FoodMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodMarket.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var loginResult = await _userService.LoginWithRoleAsync(dto.Email, dto.Password);
        if (loginResult == null)
            return Unauthorized("Неверный email или пароль");

        var (token, role) = loginResult.Value;

        Response.Cookies.Append("AccessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            message = "Успешный вход",
            role = role, // передаём реальную роль
            token = token  
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var success = await _userService.RegisterAsync(dto.Email, dto.Password);
        if (!success)
            return BadRequest("Пользователь с таким email уже существует.");

        return Ok("Регистрация прошла успешно");
    }

    [HttpGet("check")]
    //[Authorize] // ✅ Убрана политика
    public async Task<IActionResult> Check()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("Пользователь не авторизован");

        var (role, found) = await _userService.GetRoleByIdAsync(userId);
        if (!found)
            return NotFound("Роль не найдена");

        return Ok(new { role });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AccessToken");
        return Ok(new { message = "Выход выполнен" });
    }
}
