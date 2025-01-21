namespace Shared.Models;

public class CustomerData
{
    public Guid Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string VatNumber { get; set; }
    public string Email { get; set; }
    public string CompanyName { get; set; }
    public DateTime CreatedAt { get; init; }
}