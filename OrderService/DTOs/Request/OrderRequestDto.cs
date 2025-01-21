using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs.Request;

public class OrderRequestDto
{
    [Required] public required int Quantity { get; set; }
    [Required] public required Guid CustomerId { get; set; }
    public DateTime? EffectiveDate { get; set; }
    [Required] public required Guid ProductId { get; set; }
}