using ContractService.Data;
using ContractService.Models;

namespace ContractService.Repositories;

public interface IContractRepository
{
    Task<Contract> CreateContractAsync(Contract contract);
    Task SaveChangesAsync();
}

internal class ContractRepository(ContractDbContext dbContext) : IContractRepository
{
    public async Task<Contract> CreateContractAsync(Contract contract)
    {
        await dbContext.Contracts.AddAsync(contract);
        return contract;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}