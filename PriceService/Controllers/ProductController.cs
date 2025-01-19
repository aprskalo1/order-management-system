using Microsoft.AspNetCore.Mvc;
using PriceService.DTOs.Request;
using PriceService.Services;

namespace PriceService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct(ProductRequestDto productRequestDto)
    {
        var product = await productService.CreateProduct(productRequestDto);
        return Created($"api/Product/GetProductById?id={product.Id}", product);
    }

    [HttpGet("GetProductById")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await productService.GetProductById(id);
        return Ok(product);
    }
}