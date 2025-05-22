using FoodMarket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FoodMarket;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=tcp:marketdb.database.windows.net,1433;Initial Catalog=Marketdb;Persist Security Info=False;User ID=Ismail;Password=C632RRn6;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");

        return new AppDbContext(optionsBuilder.Options);
    }
}