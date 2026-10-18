using System.Collections.Generic;

namespace FoodMarket.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Category { get; set; } = null!;

        public string? NameEn { get; set; }
        public string? NameAz { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAz { get; set; }

        //public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}