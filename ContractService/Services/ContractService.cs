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
}

internal class ContractService(IContractRepository contractRepository, IMapper mapper, CustomerRpcClient customerRpcClient) : IContractService
{
    public async Task<ContractResponseDto> CreateContractAsync(ContractRequestDto contractRequestDto)
    {
        var exists = await customerRpcClient.CheckCustomerExistsAsync(contractRequestDto.CustomerId);
        if (!exists)
        {
            throw new ContractCustomerNotFoundException($"Customer with ID {contractRequestDto.CustomerId} does not exist.");
        }

        var contract = mapper.Map<Contract>(contractRequestDto);

        var returnedContract = await contractRepository.CreateContractAsync(contract);
        await contractRepository.SaveChangesAsync();

        return mapper.Map<ContractResponseDto>(returnedContract);
    }
}