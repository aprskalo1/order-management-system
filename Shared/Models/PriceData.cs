namespace Shared.Models;

public record PriceData
{
    public Guid Id { get; init; }
    public double Value { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidTo { get; init; }
    public DateTime DateIssued { get; init; }
    public Guid ProductId { get; init; }
}