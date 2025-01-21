using AutoMapper;
using Moq;
using PriceService.DTOs.Request;
using PriceService.DTOs.Response;
using PriceService.Exceptions;
using PriceService.Models;
using PriceService.Repositories;
using PriceService.Services;

namespace PriceService.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _productService = new ProductService(_productRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateProduct_Success()
        {
            // Arrange
            var productRequestDto = new ProductRequestDto
            {
                Name = "Test Product",
                Description = "Test Description"
            };

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productRequestDto.Name,
                Description = productRequestDto.Description,
                DateCreated = DateTime.UtcNow,
                Prices = []
            };

            var productResponseDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                DateCreated = product.DateCreated,
                Prices = []
            };

            _mapperMock.Setup(m => m.Map<Product>(productRequestDto)).Returns(product);
            _productRepositoryMock.Setup(r => r.CreateProduct(product)).ReturnsAsync(product);
            _productRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(productResponseDto);

            // Act
            var result = await _productService.CreateProduct(productRequestDto);

            // Assert
            Assert.Equal(productResponseDto.Id, result.Id);
            Assert.Equal(productResponseDto.Name, result.Name);
            Assert.Equal(productResponseDto.Description, result.Description);
            _productRepositoryMock.Verify(r => r.CreateProduct(product), Times.Once);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductWithPriceDateRange_ProductFound_ReturnsDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            DateTime? effectiveDate = null;
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                DateCreated = DateTime.UtcNow,
                Prices = []
            };

            var productResponseDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                DateCreated = product.DateCreated,
                Prices = []
            };

            _productRepositoryMock
                .Setup(r => r.GetProductWithPriceDateRange(productId, effectiveDate))
                .ReturnsAsync(product);
            _mapperMock
                .Setup(m => m.Map<ProductResponseDto>(product))
                .Returns(productResponseDto);

            // Act
            var result = await _productService.GetProductWithPriceDateRange(productId, effectiveDate);

            // Assert
            Assert.Equal(productResponseDto.Id, result.Id);
            Assert.Equal(productResponseDto.Name, result.Name);
            Assert.Equal(productResponseDto.Description, result.Description);
        }

        [Fact]
        public async Task GetProductWithPriceDateRange_ProductNotFound_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            DateTime? effectiveDate = null;

            _productRepositoryMock
                .Setup(r => r.GetProductWithPriceDateRange(productId, effectiveDate))
                .ReturnsAsync((Product)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(
                () => _productService.GetProductWithPriceDateRange(productId, effectiveDate)
            );
        }

        [Fact]
        public async Task GetProductById_ProductFound_ReturnsDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                DateCreated = DateTime.UtcNow,
                Prices = new List<Price>()
            };

            var productResponseDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                DateCreated = product.DateCreated,
                Prices = new List<PriceResponseDto>()
            };

            _productRepositoryMock
                .Setup(r => r.GetProductById(productId))
                .ReturnsAsync(product);
            _mapperMock
                .Setup(m => m.Map<ProductResponseDto>(product))
                .Returns(productResponseDto);

            // Act
            var result = await _productService.GetProductById(productId);

            // Assert
            Assert.Equal(productResponseDto.Id, result.Id);
            Assert.Equal(productResponseDto.Name, result.Name);
            Assert.Equal(productResponseDto.Description, result.Description);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepositoryMock
                .Setup(r => r.GetProductById(productId))
                .ReturnsAsync((Product)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(
                () => _productService.GetProductById(productId)
            );
        }
    }
}