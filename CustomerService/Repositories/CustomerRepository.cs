using CustomerService.Data;
using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Repositories;

public interface ICustomerRepository
{
    Task<Customer> CreateCustomerAsync(Customer customer);
    Task<Customer?> GetCustomerByIdAsync(Guid id);
    Task<IEnumerable<Customer>> GetCustomersAsync();
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(Customer customer);
    Task SaveChangesAsync();
}

internal class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        await dbContext.Customers.AddAsync(customer);
        return customer;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id)
    {
        return await dbContext.Customers.FindAsync(id);
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await dbContext.Customers.ToListAsync();
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        dbContext.Customers.Update(customer);
        await SaveChangesAsync();
    }

    public async Task DeleteCustomerAsync(Customer customer)
    {
        dbContext.Customers.Remove(customer);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}