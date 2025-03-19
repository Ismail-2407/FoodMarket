using FoodMarket.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodMarket.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;

        public CartController(ICartRepository repository)
        {
            _repository = repository;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _repository.GetCart(GetUserId());
            return Ok(cart);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var cart = await _repository.AddToCart(GetUserId(), productId);
            return Ok(cart);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = await _repository.RemoveFromCart(GetUserId(), productId);
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _repository.ClearCart(GetUserId());
            return NoContent();
        }
    }
}