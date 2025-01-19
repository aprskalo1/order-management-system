using Microsoft.EntityFrameworkCore;
using PriceService.Data;
using PriceService.Models;

namespace PriceService.Repositories;

public interface IPriceRepository
{
    Task<Price> CreatePrice(Price price);
    Task<Price?> GetLatestPriceByProductId(Guid productId);
    Task SaveChangesAsync();
}

internal class PriceRepository(PriceDbContext dbContext) : IPriceRepository
{
    public async Task<Price> CreatePrice(Price price)
    {
        await dbContext.Prices.AddAsync(price);
        return price;
    }

    public async Task<Price?> GetLatestPriceByProductId(Guid productId)
    {
        return await dbContext.Prices
            .OrderByDescending(p => p.ValidTo)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}