namespace PriceService.DTOs.Response;

public class PriceResponseDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public DateTime DateIssued { get; set; }
    
    public Guid ProductId { get; set; }
}