using FoodMarket.DTOs;
using FoodMarket.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodMarket.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _authService.Register(dto);
            return token == null ? BadRequest("Ошибка регистрации") : Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);
            return token == null ? Unauthorized() : Ok(new { Token = token });
        }
    }
}