using FoodMarket.Models;

namespace FoodMarket.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCart(string userId);
        Task<Cart> AddToCart(string userId, int productId);
        Task<Cart> RemoveFromCart(string userId, int productId);
        Task<bool> ClearCart(string userId);
    }
}