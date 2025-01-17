using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=.;Database=OrderData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
    }
}