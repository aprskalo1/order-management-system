using Microsoft.AspNetCore.Mvc;
using PriceService.DTOs.Request;
using PriceService.Services;

namespace PriceService.Controllers;

[ApiController]
[Route("api/products/{productId}/prices")]
public class PriceController(IPriceService priceService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePrice(PriceRequestDto priceRequestDto, Guid productId)
    {
        var price = await priceService.CreatePrice(priceRequestDto, productId);
        return Created($"api/products/{productId}/prices/{price.Id}", price);
    }
}