using System.ComponentModel.DataAnnotations;

namespace PriceService.DTOs.Request;

public class ProductRequestDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
}