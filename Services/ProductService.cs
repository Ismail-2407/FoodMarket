using FoodMarket.Data;
using FoodMarket.DTOs;
using FoodMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAll() => await _context.Products.ToListAsync();

        public async Task<Product?> GetById(int id) => await _context.Products.FindAsync(id);

        public async Task<Product> Create(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}