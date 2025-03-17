using FoodMarket.Models;

namespace FoodMarket.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrders(string userId);
        Task<Order?> GetOrderById(int id);
        Task<Order> CreateOrder(string userId, List<int> productIds);
        Task<bool> CancelOrder(int id);
    }
}