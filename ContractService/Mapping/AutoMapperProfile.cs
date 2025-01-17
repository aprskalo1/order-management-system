using AutoMapper;
using ContractService.DTOs.Request;
using ContractService.DTOs.Response;
using ContractService.Models;

namespace ContractService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Contract, ContractResponseDto>();
        CreateMap<ContractResponseDto, Contract>();

        CreateMap<Contract, ContractRequestDto>();
        CreateMap<ContractRequestDto, Contract>();
    }
}