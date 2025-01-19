using CustomerService.DTOs.Request;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpPost("AddCustomer")]
    public async Task<IActionResult> AddCustomerAsync(CustomerRequestDto customerRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await customerService.CreateCustomerAsync(customerRequestDto);
        return Created($"api/customer/{customer.Id}", customer);
    }

    [HttpGet("GetCustomerById")]
    public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
    {
        var customer = await customerService.GetCustomerByIdAsync(id);
        return Ok(customer);
    }

    [HttpGet("ListCustomers")]
    public async Task<IActionResult> ListCustomersAsync()
    {
        var customers = await customerService.GetCustomersAsync();
        return Ok(customers);
    }

    [HttpPut("UpdateCustomer")]
    public async Task<IActionResult> UpdateCustomerAsync(Guid id, CustomerRequestDto customerRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = await customerService.UpdateCustomerAsync(customerRequestDto, id);
        return Ok(customer);
    }

    [HttpDelete("DeleteCustomer")]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
        await customerService.DeleteCustomerAsync(id);
        return NoContent();
    }
}