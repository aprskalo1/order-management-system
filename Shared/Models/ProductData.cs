namespace Shared.Models;

public class ProductData
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime DateCreated { get; init; }
    public List<PriceData> Prices { get; init; }
}