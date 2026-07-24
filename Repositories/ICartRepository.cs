using FoodMarket.Models;

namespace FoodMarket.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCart(int userId);
        Task<Cart> AddToCart(int userId, int productId);
        Task<Cart> RemoveFromCart(int userId, int productId);
        Task<bool> ClearCart(int userId);
        Task<Cart> UpdateQuantity(int userId, int productId, int quantity);
    }
}