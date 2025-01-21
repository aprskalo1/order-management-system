using Microsoft.EntityFrameworkCore;
using PriceService.Data;
using PriceService.Models;

namespace PriceService.Repositories;

public interface IProductRepository
{
    Task<Product> CreateProduct(Product product);
    Task<Product?> GetProductById(Guid id);
    Task<Product?> GetProductWithPriceDateRange(Guid productId, DateTime? effectiveDate = null);
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

    public async Task<Product?> GetProductWithPriceDateRange(Guid productId, DateTime? effectiveDate = null)
    {
        var product = await dbContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null) return product;
        if (effectiveDate.HasValue)
        {
            product.Prices = product.Prices
                .Where(price => price.ValidFrom <= effectiveDate.Value && price.ValidTo >= effectiveDate.Value)
                .ToList();
        }
        else
        {
            product.Prices = product.Prices
                .OrderByDescending(price => price.DateIssued)
                .Take(1)
                .ToList();
        }

        return product;
    }


    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}