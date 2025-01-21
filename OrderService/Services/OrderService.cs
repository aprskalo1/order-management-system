using AutoMapper;
using OrderService.DTOs.Request;
using OrderService.DTOs.Response;
using OrderService.Exceptions;
using OrderService.Messaging;
using OrderService.Models;
using OrderService.Repositories;
using Shared.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto order);
}

internal class OrderService(
    IOrderRepository orderRepository,
    IMapper mapper,
    ContractRpcClient contractRpcClient,
    ProductRpcClient productRpcClient)
    : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequestDto)
    {
        var contract = await contractRpcClient.GetLastContractAsync(orderRequestDto.CustomerId);

        var product = await productRpcClient.GetProductAsync(orderRequestDto.ProductId, orderRequestDto.EffectiveDate ?? DateTime.Now);
        if (product is null)
        {
            throw new OrderCreationException($"Cannot create order. Product with Id = {orderRequestDto.ProductId} does not exist.");
        }

        var order = mapper.Map<Order>(orderRequestDto);

        if (contract != null)
        {
            order.FinalPrice = CalculateContractPrice(product, orderRequestDto.Quantity, contract);
        }
        else
        {
            order.FinalPrice = product.Prices.FirstOrDefault()!.Value * orderRequestDto.Quantity;
        }

        if (product.Prices.Count == 0)
        {
            throw new OrderCreationException("No pricing information available for the product at this date range.");
        }

        order.ProductName = product.Name;
        order.ProductId = product.Id;

        await orderRepository.CreateOrderAsync(order);
        await orderRepository.SaveChangesAsync();

        return mapper.Map<OrderResponseDto>(order);
    }

    private double CalculateContractPrice(ProductData product, int quantity, ContractData contract)
    {
        var price = product.Prices.FirstOrDefault();
        if (price == null)
        {
            throw new OrderCreationException("No pricing information available for the product at this date range.");
        }

        return price.Value * quantity * (1 - contract.DiscountRate / 100);
    }
}