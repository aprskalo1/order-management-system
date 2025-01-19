using Microsoft.EntityFrameworkCore;
using PriceService.Models;

namespace PriceService.Data;

public class PriceDbContext(DbContextOptions<PriceDbContext> options) : DbContext(options)
{
    public DbSet<Price> Prices { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=.;Database=PriceData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
    }
}