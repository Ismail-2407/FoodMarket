using FoodMarket.Models;
using FoodMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var token = await _authService.Register(request.Email, request.Password);
        if (token == null)
            return BadRequest("Регистрация не удалась");

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var token = await _authService.Login(request.Email, request.Password);
        if (token == null)
            return Unauthorized("Неверные данные");

        return Ok(new { token });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-check")]
    public IActionResult AdminCheck()
    {
        return Ok("Вы — админ");
    }
}

public class AuthRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}