using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodMarket.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrders(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrder(string userId, List<int> productIds)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId is null or empty");

            if (productIds == null || productIds.Count == 0)
                throw new ArgumentException("ProductIds cannot be empty");

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count == 0)
                throw new ArgumentException("No valid products found");

            var orderItems = products.Select(p => new OrderItem
            {
                ProductId = p.Id,
                Quantity = 1
            }).ToList();

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems,
                TotalPrice = products.Sum(p => p.Price),
                Status = "Pending"
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
