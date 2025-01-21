using ContractService.Data;
using ContractService.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Repositories;

public interface IContractRepository
{
    Task<Contract> CreateContractAsync(Contract contract);
    Task<Contract?> GetLastContractAsync(Guid customerId);
    Task SaveChangesAsync();
}

internal class ContractRepository(ContractDbContext dbContext) : IContractRepository
{
    public async Task<Contract> CreateContractAsync(Contract contract)
    {
        await dbContext.Contracts.AddAsync(contract);
        return contract;
    }

    public async Task<Contract?> GetLastContractAsync(Guid customerId)
    {
        return await dbContext.Contracts
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.DateIssued)
            .FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}