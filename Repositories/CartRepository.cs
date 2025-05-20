using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> AddToCart(string userId, int productId)
        {
            var cart = await GetCart(userId);
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return cart;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                });
            }

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveFromCart(string userId, int productId)
        {
            var cart = await GetCart(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cart = await GetCart(userId);
            cart.Items.Clear();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}