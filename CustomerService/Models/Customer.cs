using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models;

public class Customer
{
    [Key] public Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? VatNumber { get; set; }
    public string? CompanyName { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}