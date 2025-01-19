using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs.Request;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderRequestDto orderRequestDto)
    {
        var order = await orderService.CreateOrderAsync(orderRequestDto);
        return Created($"/api/order/{order.Id}", order);
    }
}