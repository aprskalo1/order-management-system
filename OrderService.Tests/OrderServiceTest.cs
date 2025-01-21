using AutoMapper;
using Moq;
using OrderService.DTOs.Request;
using OrderService.DTOs.Response;
using OrderService.Exceptions;
using OrderService.Models;
using OrderService.Repositories;
using OrderService.Services;
using OrderService.Messaging;
using Shared.Models;

namespace OrderService.Tests;

public class OrderServiceTest
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ContractRpcClient> _contractRpcClientMock;
    private readonly Mock<ProductRpcClient> _productRpcClientMock;
    private readonly IOrderService _orderService;

    public OrderServiceTest()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _contractRpcClientMock = new Mock<ContractRpcClient>();
        _productRpcClientMock = new Mock<ProductRpcClient>();

        _orderService = new Services.OrderService(
            _orderRepositoryMock.Object,
            _mapperMock.Object,
            _contractRpcClientMock.Object,
            _productRpcClientMock.Object
        );
    }

    [Fact]
    public async Task CreateOrderAsync_Success()
    {
        // Arrange
        var orderRequestDto = new OrderRequestDto
        {
            Quantity = 2,
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EffectiveDate = DateTime.UtcNow
        };

        var contractData = new ContractData
        {
            Id = Guid.NewGuid(),
            CustomerId = orderRequestDto.CustomerId,
            DiscountRate = 10,
            DateIssued = DateTime.UtcNow
        };

        var priceData = new PriceData { Value = 100 };
        var productData = new ProductData
        {
            Id = orderRequestDto.ProductId,
            Name = "Test Product",
            Description = "A product for testing",
            DateCreated = DateTime.UtcNow,
            Prices = [priceData]
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = orderRequestDto.CustomerId,
            ProductId = productData.Id,
            ProductName = productData.Name,
            Quantity = orderRequestDto.Quantity,
            EffectiveDate = orderRequestDto.EffectiveDate ?? DateTime.Now,
            FinalPrice = 0.0 // Initialized to satisfy required member constraint
        };

        // Setup mocks
        _contractRpcClientMock
            .Setup(x => x.GetLastContractAsync(orderRequestDto.CustomerId))
            .ReturnsAsync(contractData);

        _productRpcClientMock
            .Setup(x => x.GetProductAsync(orderRequestDto.ProductId, It.IsAny<DateTime>()))
            .ReturnsAsync(productData);

        _mapperMock
            .Setup(m => m.Map<Order>(orderRequestDto))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.CreateOrderAsync(order))
            .ReturnsAsync(order);

        _orderRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var orderResponseDto = new OrderResponseDto
        {
            Id = order.Id,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            FinalPrice = 180,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            EffectiveDate = order.EffectiveDate,
            DateIssued = order.DateIssued
        };

        _mapperMock
            .Setup(m => m.Map<OrderResponseDto>(order))
            .Returns(orderResponseDto);

        // Act
        var result = await _orderService.CreateOrderAsync(orderRequestDto);

        // Assert
        Assert.Equal(orderResponseDto.Id, result.Id);
        Assert.Equal(orderResponseDto.FinalPrice, result.FinalPrice);
        Assert.Equal(orderResponseDto.ProductName, result.ProductName);
    }

    [Fact]
    public async Task CreateOrderAsync_ProductNotFound_ThrowsException()
    {
        // Arrange
        var orderRequestDto = new OrderRequestDto
        {
            Quantity = 1,
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EffectiveDate = DateTime.UtcNow
        };

        _contractRpcClientMock
            .Setup(x => x.GetLastContractAsync(orderRequestDto.CustomerId))
            .ReturnsAsync(new ContractData
            {
                Id = Guid.NewGuid(),
                CustomerId = orderRequestDto.CustomerId,
                DiscountRate = 0,
                DateIssued = DateTime.UtcNow
            });

        _productRpcClientMock
            .Setup(x => x.GetProductAsync(orderRequestDto.ProductId, It.IsAny<DateTime>()))
            .ReturnsAsync((ProductData)null!);

        // Act & Assert
        await Assert.ThrowsAsync<OrderCreationException>(() => _orderService.CreateOrderAsync(orderRequestDto));
    }
}