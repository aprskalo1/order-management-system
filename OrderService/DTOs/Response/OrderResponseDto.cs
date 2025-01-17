namespace OrderService.DTOs.Response;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal FinalPrice { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime DateIssued { get; set; }
}