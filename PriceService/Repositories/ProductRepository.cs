using Microsoft.EntityFrameworkCore;
using PriceService.Data;
using PriceService.Models;

namespace PriceService.Repositories;

public interface IProductRepository
{
    Task<Product> CreateProduct(Product product);
    Task<Product?> GetProductById(Guid id);
    Task SaveChangesAsync();
}

internal class ProductRepository(PriceDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProduct(Product product)
    {
        await dbContext.Products.AddAsync(product);
        return product;
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        return await dbContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}