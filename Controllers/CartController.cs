// using FoodMarket.Repositories;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;
//
// namespace FoodMarket.Controllers
// {
//     [ApiController]
//     [Route("api/cart")]
//     [Authorize]
//     public class CartController : ControllerBase
//     {
//         private readonly ICartRepository _cartRepository;
//
//         public CartController(ICartRepository cartRepository)
//         {
//             _cartRepository = cartRepository;
//         }
//
//         private string? GetUserId()
//         {
//             Console.WriteLine("GetUserId");
//             Console.WriteLine(User.FindFirstValue(ClaimTypes.NameIdentifier));
//             return User.FindFirstValue(ClaimTypes.NameIdentifier);
//         }
//
//         [HttpGet]
//         public async Task<IActionResult> GetCart()
//         {
//             var userId = GetUserId();
//             if (string.IsNullOrEmpty(userId))
//                 return Unauthorized("User ID not found in token.");
//
//             var cart = await _cartRepository.GetCart(userId);
//
//             var result = cart.Items.Select(i => new
//             {
//                 i.ProductId,
//                 i.Product.Name,
//                 i.Product.Price,
//                 i.Product.ImageUrl,
//                 i.Quantity
//             });
//
//             return Ok(result);
//         }
//
//         [HttpPost("add/{productId}")]
//         public async Task<IActionResult> AddToCart(int productId)
//         {
//             var usr = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             
//             // if (string.IsNullOrEmpty(userId))
//             //     return Unauthorized("User ID not found in token.");
//
//             if (usr == null)
//             {
//                 return Unauthorized("User ID not found in token.");
//             }
//             var cart = await _cartRepository.AddToCart(usr, productId);
//             return Ok(cart);
//         }
//
//
//
//         [HttpDelete("remove/{productId}")]
//         [Authorize(Policy = "AdminPolicy")]
//         public async Task<IActionResult> RemoveFromCart(int productId)
//         {
//             var userId = GetUserId();
//             if (string.IsNullOrEmpty(userId))
//                 return Unauthorized("User ID not found in token.");
//
//             var cart = await _cartRepository.RemoveFromCart(userId, productId);
//             return Ok(cart);
//         }
//
//         [HttpDelete("clear")]
//         public async Task<IActionResult> ClearCart()
//         {
//             var userId = GetUserId();
//             if (string.IsNullOrEmpty(userId))
//                 return Unauthorized("User ID not found in token.");
//
//             var result = await _cartRepository.ClearCart(userId);
//             return Ok(result);
//         }
//     }
// }