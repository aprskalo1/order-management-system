namespace ContractService.DTOs.Response;

public class ContractResponseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public double DiscountRate { get; set; }
    public DateTime DateIssued { get; set; }
}