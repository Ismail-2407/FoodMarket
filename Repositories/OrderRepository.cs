using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrders(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Products)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrder(string userId, List<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var order = new Order
            {
                UserId = userId,
                Products = products,
                TotalPrice = products.Sum(p => p.Price)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status == "Completed") return false;

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}