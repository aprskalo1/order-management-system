namespace Shared.Models;

public record ContractData
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public double DiscountRate { get; init; }
    public DateTime DateIssued { get; init; }
}