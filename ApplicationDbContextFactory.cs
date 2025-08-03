using FoodMarket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FoodMarket;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // ПРИМЕЧАНИЕ: эта строка подключения должна быть PostgreSQL, НЕ SQL Server
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=foodmarket_db;Username=postgres;Password=secret");

        return new AppDbContext(optionsBuilder.Options);
    }
}
