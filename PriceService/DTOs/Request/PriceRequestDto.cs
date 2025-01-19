using System.ComponentModel.DataAnnotations;

namespace PriceService.DTOs.Request;

public class PriceRequestDto
{
    [Required] public decimal Value { get; set; }
    [Required] public DateTime ValidFrom { get; set; }
    [Required] public DateTime ValidTo { get; set; }
    
    [Required] public Guid ProductId { get; set; }
}