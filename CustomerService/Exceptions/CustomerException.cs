namespace CustomerService.Exceptions;

internal class CustomerNotFoundException(string message) : CustomerCustomException(message)
{
    public override string ToString()
    {
        return $"CustomerException: {Message}";
    }
}