using FoodMarket.DTOs;
using FoodMarket.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodMarket.Controllers
{
    [Route("api/orders")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repository;

        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _repository.GetOrders(GetUserId());
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _repository.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto dto)
        {
            var order = await _repository.CreateOrder(GetUserId(), dto.ProductIds);
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