using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

public class Order
{
    [Key] public Guid Id { get; set; }
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    [Precision(18, 2)] public required decimal FinalPrice { get; set; }
    public required Guid CustomerId { get; set; }
    public DateTime DateIssued { get; set; } = DateTime.Now;
}