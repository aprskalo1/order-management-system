using AutoMapper;
using PriceService.DTOs.Request;
using PriceService.DTOs.Response;
using PriceService.Models;

namespace PriceService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Price, PriceResponseDto>();
        CreateMap<PriceRequestDto, Price>();

        CreateMap<PriceRequestDto, Price>();
        CreateMap<Price, PriceResponseDto>();

        CreateMap<Product, ProductResponseDto>();
        CreateMap<ProductResponseDto, Product>();

        CreateMap<ProductRequestDto, Product>();
        CreateMap<Product, ProductResponseDto>();
    }
}