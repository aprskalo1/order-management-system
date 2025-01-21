namespace OrderService.Exceptions;

internal class OrderCreationException(string message) : OrderCustomException(message)
{
    public override string ToString()
    {
        return $"OrderCreationException: {Message}";
    }
}