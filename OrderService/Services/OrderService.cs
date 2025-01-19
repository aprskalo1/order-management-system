using AutoMapper;
using OrderService.DTOs.Request;
using OrderService.DTOs.Response;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto order);
}

internal class OrderService(IOrderRepository orderRepository, IMapper mapper) : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequestDto)
    {
        var order = mapper.Map<Order>(orderRequestDto);

        await orderRepository.CreateOrderAsync(order);
        await orderRepository.SaveChangesAsync();

        return mapper.Map<OrderResponseDto>(order);
    }
}