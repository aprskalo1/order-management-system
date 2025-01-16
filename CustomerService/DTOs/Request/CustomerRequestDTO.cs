using System.ComponentModel.DataAnnotations;

namespace CustomerService.DTOs.Request;

public class CustomerRequestDto
{
    [Required] public required string FirstName { get; set; }
    [Required] public required string LastName { get; set; }
    [Required] public required string Email { get; set; }
    public string? VatNumber { get; set; }
    public string? CompanyName { get; set; }
}