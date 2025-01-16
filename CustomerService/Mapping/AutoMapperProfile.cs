using AutoMapper;
using CustomerService.DTOs.Request;
using CustomerService.DTOs.Response;
using CustomerService.Models;

namespace CustomerService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Customer, CustomerResponseDto>();
        CreateMap<CustomerResponseDto, Customer>();

        CreateMap<Customer, CustomerRequestDto>();
        CreateMap<CustomerRequestDto, Customer>();
    }
}