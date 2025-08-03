using FoodMarket.DTOs;
using FoodMarket.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodMarket.Services;

namespace FoodMarket.Controllers
{
    [Route("api/orders")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IUserService _userService;

        public OrderController(IOrderRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _repository.GetOrders(GetUserId());
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute]int id)
        {
            var order = await _repository.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto dto)
        {
            var userId = dto.UserId.ToString();
            if (userId == null)
                return Unauthorized("User not found");

            if (dto.ProductIds == null || !dto.ProductIds.Any())
                return BadRequest("ProductIds cannot be empty");

            var order = await _repository.CreateOrder(userId, dto.ProductIds);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _repository.CancelOrder(id);
            return result ? NoContent() : BadRequest("Cannot cancel this order");
        }
    }
}