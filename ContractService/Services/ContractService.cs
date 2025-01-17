using AutoMapper;
using ContractService.DTOs.Request;
using ContractService.DTOs.Response;
using ContractService.Models;
using ContractService.Repositories;

namespace ContractService.Services;

public interface IContractService
{
    Task<ContractResponseDto> CreateContractAsync(ContractRequestDto contractRequestDto);
}

internal class ContractService(IContractRepository contractRepository, IMapper mapper) : IContractService
{
    public async Task<ContractResponseDto> CreateContractAsync(ContractRequestDto contractRequestDto)
    {
        var contract = mapper.Map<Contract>(contractRequestDto);

        var returnedContract = await contractRepository.CreateContractAsync(contract);
        await contractRepository.SaveChangesAsync();

        return mapper.Map<ContractResponseDto>(returnedContract);
    }
}