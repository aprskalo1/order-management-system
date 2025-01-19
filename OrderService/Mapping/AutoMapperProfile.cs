using AutoMapper;
using OrderService.DTOs.Request;
using OrderService.DTOs.Response;
using OrderService.Models;

namespace OrderService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, OrderResponseDto>();
        CreateMap<OrderResponseDto, Order>();

        CreateMap<Order, OrderRequestDto>();
        CreateMap<OrderRequestDto, Order>();
    }
}