using Microsoft.AspNetCore.Mvc;
using PriceService.DTOs.Request;
using PriceService.Services;

namespace PriceService.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> CreateProduct(ProductRequestDto productRequestDto)
    {
        var product = await productService.CreateProduct(productRequestDto);
        return Created($"api/product/{product.Id}", product);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await productService.GetProductById(id);
        return Ok(product);
    }

    [HttpGet("{productId:guid}/price")]
    public async Task<IActionResult> GetProductWithPriceDateRange(Guid productId, DateTime? effectiveDate = null)
    {
        var product = await productService.GetProductWithPriceDateRange(productId, effectiveDate);
        return Ok(product);
    }
}