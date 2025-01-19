namespace PriceService.Exceptions;

internal class ProductNotFoundException(string message) : ProductCustomException(message)
{
    public override string ToString()
    {
        return $"ProductException: {Message}";
    }
}

internal class PriceDateCorrelationException(string message) : ProductCustomException(message)
{
    public override string ToString()
    {
        return $"PriceDateCorrelationException: {Message}";
    }
}