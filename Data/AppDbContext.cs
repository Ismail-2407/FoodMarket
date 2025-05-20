using FoodMarket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CartItem>()
                .HasOne<FoodMarket.Models.Cart>(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            builder.Entity<CartItem>()
                .HasOne<FoodMarket.Models.Product>(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            builder.Entity<FoodMarket.Models.Product>()
                .Property<decimal>(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<FoodMarket.Models.Product>().HasData(
                new Product { Id = 1, Name = "Яблоко", Description = "Свежее красное яблоко", Price = 1.99m, ImageUrl = "/images/apple.jpg", Category = "Фрукты" },
                new Product { Id = 2, Name = "Морковь", Description = "Сочная морковь", Price = 0.89m, ImageUrl = "/images/carrot.jpg", Category = "Овощи" },
                new Product { Id = 3, Name = "Хлеб", Description = "Свежий пшеничный хлеб", Price = 1.49m, ImageUrl = "/images/bread.jpg", Category = "Выпечка" },
                new Product { Id = 4, Name = "Молоко", Description = "Натуральное молоко 1л", Price = 1.29m, ImageUrl = "/images/milk.jpg", Category = "Напитки" },
                new Product { Id = 5, Name = "Банан", Description = "Спелый желтый банан", Price = 1.19m, ImageUrl = "/images/banana.jpg", Category = "Фрукты" },
                new Product { Id = 6, Name = "Картофель", Description = "Свежий картофель", Price = 0.69m, ImageUrl = "/images/potato.jpg", Category = "Овощи" },
                new Product { Id = 7, Name = "Сыр", Description = "Твердый сыр 200г", Price = 2.99m, ImageUrl = "/images/cheese.jpg", Category = "Выпечка" },
                new Product { Id = 8, Name = "Сок апельсиновый", Description = "Свежевыжатый апельсиновый сок 1л", Price = 1.59m, ImageUrl = "/images/orange-juice.jpg", Category = "Напитки" },
                new Product { Id = 9, Name = "Клубника", Description = "Свежая красная клубника", Price = 2.49m, ImageUrl = "/images/strawberry.jpg", Category = "Фрукты" },
                new Product { Id = 10, Name = "Цветная капуста", Description = "Свежая цветная капуста", Price = 1.79m, ImageUrl = "/images/cauliflower.jpg", Category = "Овощи" },
                new Product { Id = 11, Name = "Булка", Description = "Свежая белая булка", Price = 0.89m, ImageUrl = "/images/roll.jpg", Category = "Выпечка" },
                new Product { Id = 12, Name = "Кефир", Description = "Натуральный кефир 1л", Price = 1.09m, ImageUrl = "/images/kefir.jpg", Category = "Напитки" }
            );
        }
    }
}
