using AutoMapper;
using Moq;
using PriceService.DTOs.Request;
using PriceService.DTOs.Response;
using PriceService.Exceptions;
using PriceService.Models;
using PriceService.Repositories;
using PriceService.Services;

namespace PriceService.Tests;

public class PriceServiceTests
{
    private readonly Mock<IPriceRepository> _priceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IPriceService _priceService;

    public PriceServiceTests()
    {
        _priceRepositoryMock = new Mock<IPriceRepository>();
        _mapperMock = new Mock<IMapper>();
        _priceService = new Services.PriceService(_priceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreatePrice_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var priceRequestDto = new PriceRequestDto
        {
            Value = 100,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(10),
            ProductId = productId
        };

        //  no existing price for the product.
        Price currentPrice = null;

        var dummyProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description"
        };

        var price = new Price
        {
            Id = Guid.NewGuid(),
            Value = priceRequestDto.Value,
            ValidFrom = priceRequestDto.ValidFrom,
            ValidTo = priceRequestDto.ValidTo,
            ProductId = productId,
            Product = dummyProduct,
            DateIssued = DateTime.UtcNow
        };

        var priceResponseDto = new PriceResponseDto
        {
            Id = price.Id,
            Value = price.Value,
            ValidFrom = price.ValidFrom,
            ValidTo = price.ValidTo,
            DateIssued = price.DateIssued,
            ProductId = price.ProductId
        };

        _priceRepositoryMock.Setup(r => r.GetLatestPriceByProductId(productId)).ReturnsAsync(currentPrice);
        _mapperMock.Setup(m => m.Map<Price>(priceRequestDto)).Returns(price);
        _priceRepositoryMock.Setup(r => r.CreatePrice(price)).ReturnsAsync(price);

        _priceRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<PriceResponseDto>(price)).Returns(priceResponseDto);

        // Act
        var result = await _priceService.CreatePrice(priceRequestDto, productId);

        // Assert
        Assert.Equal(priceResponseDto.Id, result.Id);
        Assert.Equal(priceResponseDto.Value, result.Value);
        Assert.Equal(priceResponseDto.ValidFrom, result.ValidFrom);
        Assert.Equal(priceResponseDto.ValidTo, result.ValidTo);
        Assert.Equal(priceResponseDto.ProductId, result.ProductId);

        _priceRepositoryMock.Verify(r => r.CreatePrice(price), Times.Once);
        _priceRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePrice_InvalidDateCorrelation_ThrowsException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var priceRequestDto = new PriceRequestDto
        {
            Value = 100,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(10),
            ProductId = productId
        };

        var dummyProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description"
        };

        var currentPrice = new Price
        {
            Id = Guid.NewGuid(),
            Value = 90,
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ValidTo = DateTime.UtcNow.AddDays(2),
            ProductId = productId,
            Product = dummyProduct,
            DateIssued = DateTime.UtcNow
        };

        _priceRepositoryMock.Setup(r => r.GetLatestPriceByProductId(productId)).ReturnsAsync(currentPrice);

        // Act & Assert
        await Assert.ThrowsAsync<PriceDateCorrelationException>(
            () => _priceService.CreatePrice(priceRequestDto, productId)
        );
    }
}