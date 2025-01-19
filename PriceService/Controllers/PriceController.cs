using Microsoft.AspNetCore.Mvc;
using PriceService.DTOs.Request;
using PriceService.Services;

namespace PriceService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceController(IPriceService priceService) : ControllerBase
{
    [HttpPost("CreatePrice")]
    public async Task<IActionResult> CreatePrice(PriceRequestDto priceRequestDto, Guid productId)
    {
        var price = await priceService.CreatePrice(priceRequestDto, productId);
        return Created($"api/Price/GetPriceById?id={price.Id}", price);
    }
}