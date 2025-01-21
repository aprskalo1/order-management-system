namespace CustomerService.Exceptions;

public class CustomerNotFoundException(string message) : CustomerCustomException(message)
{
    public override string ToString()
    {
        return $"CustomerException: {Message}";
    }
}