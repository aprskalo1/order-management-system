using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs.Request;

public class OrderRequestDto
{
    [Required] public required string ProductName { get; set; }
    [Required] public required int Quantity { get; set; }
    [Required] public required decimal FinalPrice { get; set; }
    [Required] public required Guid CustomerId { get; set; }
}