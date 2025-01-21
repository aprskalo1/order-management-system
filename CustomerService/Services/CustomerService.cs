using AutoMapper;
using CustomerService.DTOs.Request;
using CustomerService.DTOs.Response;
using CustomerService.Exceptions;
using CustomerService.Models;
using CustomerService.Repositories;

namespace CustomerService.Services;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateCustomerAsync(CustomerRequestDto customerRequestDto);
    Task<CustomerResponseDto> GetCustomerByIdAsync(Guid id);
    Task<IEnumerable<CustomerResponseDto>> GetCustomersAsync();
    Task<CustomerResponseDto> UpdateCustomerAsync(CustomerRequestDto customerRequestDto, Guid id);
    Task DeleteCustomerAsync(Guid id);
}

public class CustomerService(ICustomerRepository customerRepository, IMapper mapper) : ICustomerService
{
    public async Task<CustomerResponseDto> CreateCustomerAsync(CustomerRequestDto customerRequestDto)
    {
        var customer = mapper.Map<Customer>(customerRequestDto);

        await customerRepository.CreateCustomerAsync(customer);
        await customerRepository.SaveChangesAsync();

        return mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto> GetCustomerByIdAsync(Guid id)
    {
        var customer = await CheckAndReturnCustomer(id);
        return mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetCustomersAsync()
    {
        var customers = await customerRepository.GetCustomersAsync();

        if (customers == null || !customers.Any())
            throw new CustomerNotFoundException("Oops! No customers found.");

        return mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
    }

    public async Task<CustomerResponseDto> UpdateCustomerAsync(CustomerRequestDto customerRequestDto, Guid id)
    {
        var customer = await CheckAndReturnCustomer(id);

        mapper.Map(customerRequestDto, customer);
        await customerRepository.UpdateCustomerAsync(customer);
        await customerRepository.SaveChangesAsync();

        return mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        var customer = await CheckAndReturnCustomer(id);
        await customerRepository.DeleteCustomerAsync(customer);
        await customerRepository.SaveChangesAsync();
    }

    private async Task<Customer> CheckAndReturnCustomer(Guid id)
    {
        var customer = await customerRepository.GetCustomerByIdAsync(id);

        if (customer == null)
            throw new CustomerNotFoundException($"Oops! Customer with id: {id} not found.");

        return customer;
    }
}