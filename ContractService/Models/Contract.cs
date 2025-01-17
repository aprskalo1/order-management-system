using System.ComponentModel.DataAnnotations;

namespace ContractService.Models;

public class Contract
{
    [Key]
    public Guid Id { get; set; }
    public required Guid CustomerId { get; set; }
    public required double DiscountRate { get; set; }
    public required DateTime ValidTo { get; set; }
    public DateTime DateIssued { get; set; } = DateTime.Now;
}