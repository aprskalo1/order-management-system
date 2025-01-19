using AutoMapper;
using PriceService.DTOs.Request;
using PriceService.DTOs.Response;
using PriceService.Exceptions;
using PriceService.Models;
using PriceService.Repositories;

namespace PriceService.Services;

public interface IPriceService
{
    Task<PriceResponseDto> CreatePrice(PriceRequestDto priceRequestDto, Guid productId);
}

internal class PriceService(IPriceRepository priceRepository, IMapper mapper) : IPriceService
{
    public async Task<PriceResponseDto> CreatePrice(PriceRequestDto priceRequestDto, Guid productId)
    {
        var currentPrice = await priceRepository.GetLatestPriceByProductId(productId);

        if (currentPrice != null && priceRequestDto.ValidFrom < currentPrice.ValidTo)
        {
            throw new PriceDateCorrelationException("The price valid from date must be after the current price valid to date");
        }

        var price = mapper.Map<Price>(priceRequestDto);
        price.ProductId = productId;

        await priceRepository.CreatePrice(price);
        await priceRepository.SaveChangesAsync();

        return mapper.Map<PriceResponseDto>(price);
    }
}