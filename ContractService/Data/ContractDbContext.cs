using ContractService.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Data;

public class ContractDbContext(DbContextOptions<ContractDbContext> options) : DbContext(options)
{
    public DbSet<Contract> Contracts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=.;Database=ContractData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
    }
}