using System.ComponentModel.DataAnnotations;

namespace PriceService.Models;

public class Product
{
    [Key] public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;

    public List<Price> Prices { get; set; }
}