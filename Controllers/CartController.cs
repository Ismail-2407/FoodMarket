using FoodMarket.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodMarket.Controllers
{
    [ApiController]
    [Route("api/cart")]
    // [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        private int? GetUserId()
        {
            // ✅ Читаем токен из заголовка вручную
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userIdStr = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdStr, out var userId))
                    return userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading token: {ex.Message}");
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartRepository.GetCart(userId.Value);

            var result = cart.Items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.ImageUrl,
                i.Quantity
            });

            return Ok(result);
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartRepository.AddToCart(userId.Value, productId);

            var result = cart.Items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.ImageUrl,
                i.Quantity
            });

            return Ok(result);
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartRepository.RemoveFromCart(userId.Value, productId);

            var result = cart.Items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.ImageUrl,
                i.Quantity
            });

            return Ok(result);
        }

        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] int quantity)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartRepository.UpdateQuantity(userId.Value, productId, quantity);

            var result = cart.Items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.ImageUrl,
                i.Quantity
            });

            return Ok(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            await _cartRepository.ClearCart(userId.Value);
            return Ok(new { message = "Корзина очищена" });
        }
    }
}