using AutoMapper;
using PriceService.DTOs.Request;
using PriceService.DTOs.Response;
using PriceService.Exceptions;
using PriceService.Models;
using PriceService.Repositories;

namespace PriceService.Services;

public interface IProductService
{
    Task<ProductResponseDto> CreateProduct(ProductRequestDto productRequestDto);

    Task<ProductResponseDto> GetProductWithPriceDateRange(Guid productId, DateTime? effectiveDate = null);
    Task<ProductResponseDto> GetProductById(Guid id);
}

public class ProductService(IProductRepository productRepository, IMapper mapper) : IProductService
{
    public async Task<ProductResponseDto> CreateProduct(ProductRequestDto productRequestDto)
    {
        var product = mapper.Map<Product>(productRequestDto);

        await productRepository.CreateProduct(product);
        await productRepository.SaveChangesAsync();

        return mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> GetProductWithPriceDateRange(Guid productId, DateTime? effectiveDate = null)
    {
        var product = await productRepository.GetProductWithPriceDateRange(productId, effectiveDate);
        if (product == null)
        {
            throw new ProductNotFoundException($"Product with id {productId} not found");
        }

        var productDto = mapper.Map<ProductResponseDto>(product);
        return productDto;
    }

    public async Task<ProductResponseDto> GetProductById(Guid id)
    {
        var product = await productRepository.GetProductById(id);
        if (product == null)
        {
            throw new ProductNotFoundException($"Product with id {id} not found");
        }

        var productDto = mapper.Map<ProductResponseDto>(product);
        return productDto;
    }
}