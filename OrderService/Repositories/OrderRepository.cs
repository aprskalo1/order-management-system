using OrderService.Data;
using OrderService.Models;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task SaveChangesAsync();
}

internal class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<Order> CreateOrderAsync(Order order)
    {
        await dbContext.Orders.AddAsync(order);
        return order;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}