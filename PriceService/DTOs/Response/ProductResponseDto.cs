namespace PriceService.DTOs.Response;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime DateCreated { get; set; }

    public required List<PriceResponseDto> Prices { get; set; }
}