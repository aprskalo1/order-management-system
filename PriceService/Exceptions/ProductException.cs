namespace PriceService.Exceptions;

public class ProductNotFoundException(string message) : ProductCustomException(message)
{
    public override string ToString()
    {
        return $"ProductException: {Message}";
    }
}

public class PriceDateCorrelationException(string message) : ProductCustomException(message)
{
    public override string ToString()
    {
        return $"PriceDateCorrelationException: {Message}";
    }
}