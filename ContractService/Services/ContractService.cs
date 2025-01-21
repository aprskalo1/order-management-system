using AutoMapper;
using ContractService.DTOs.Request;
using ContractService.DTOs.Response;
using ContractService.Exceptions;
using ContractService.Messaging.RPC;
using ContractService.Models;
using ContractService.Repositories;

namespace ContractService.Services;

public interface IContractService
{
    Task<ContractResponseDto> CreateContractAsync(ContractRequestDto contractRequestDto);
    Task<ContractResponseDto> GetLastContractAsync(Guid customerId);
}

public class ContractService(IContractRepository contractRepository, IMapper mapper, CustomerRpcClient customerRpcClient) : IContractService
{
    public async Task<ContractResponseDto> CreateContractAsync(ContractRequestDto contractRequestDto)
    {
        var customer = await customerRpcClient.GetCustomerAsync(contractRequestDto.CustomerId);
        if (customer is null)
        {
            throw new ContractCustomerNotFoundException($"Customer with ID {contractRequestDto.CustomerId} does not exist.");
        }

        var contract = mapper.Map<Contract>(contractRequestDto);

        var returnedContract = await contractRepository.CreateContractAsync(contract);
        await contractRepository.SaveChangesAsync();

        return mapper.Map<ContractResponseDto>(returnedContract);
    }

    public async Task<ContractResponseDto> GetLastContractAsync(Guid customerId)
    {
        var contract = await contractRepository.GetLastContractAsync(customerId);
        if (contract is null)
        {
            throw new ContractNotFoundException($"Contract for customer with ID {customerId} does not exist.");
        }

        return mapper.Map<ContractResponseDto>(contract);
    }
}