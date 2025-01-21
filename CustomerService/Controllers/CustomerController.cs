using CustomerService.DTOs.Request;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCustomerAsync(CustomerRequestDto customerRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await customerService.CreateCustomerAsync(customerRequestDto);
        return Created($"/api/customers/{customer.Id}", customer);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
    {
        var customer = await customerService.GetCustomerByIdAsync(id);
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> ListCustomersAsync()
    {
        var customers = await customerService.GetCustomersAsync();
        return Ok(customers);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomerAsync(Guid id, CustomerRequestDto customerRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await customerService.UpdateCustomerAsync(customerRequestDto, id);
        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
        await customerService.DeleteCustomerAsync(id);
        return NoContent();
    }
}