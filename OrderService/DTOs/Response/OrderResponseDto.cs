using Microsoft.EntityFrameworkCore;

namespace OrderService.DTOs.Response;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    [Precision(18, 2)] public double FinalPrice { get; set; }
    public Guid CustomerId { get; set; }
    public required Guid ProductId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime DateIssued { get; set; }
}