using FoodMarket.DTOs;
using FoodMarket.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodMarket.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task<Product> Create(ProductDto dto);
    }
}