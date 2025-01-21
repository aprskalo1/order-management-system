using System.ComponentModel.DataAnnotations;

namespace ContractService.DTOs.Request;

public class ContractRequestDto
{
    [Required] public required Guid CustomerId { get; set; }
    [Required] public required double DiscountRate { get; set; }
}