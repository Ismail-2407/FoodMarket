using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCart(string userId)
        {
            return await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart { UserId = userId };
        }

        public async Task<Cart> AddToCart(string userId, int productId)
        {
            var cart = await GetCart(userId);
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return cart;

            cart.Products.Add(product);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveFromCart(string userId, int productId)
        {
            var cart = await GetCart(userId);
            var product = cart.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                cart.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cart = await GetCart(userId);
            cart.Products.Clear();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}