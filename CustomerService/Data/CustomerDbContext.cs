using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=.;Database=CustomerData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
    }
}