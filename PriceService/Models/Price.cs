using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PriceService.Models;

public class Price
{
    [Key] public Guid Id { get; set; }
    [Precision(18, 2)] public decimal Value { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public DateTime DateIssued { get; set; } = DateTime.Now;

    public Guid ProductId { get; set; }
    public required Product Product { get; set; }
}