namespace ContractService.Exceptions;

public class ContractNotFoundException(string message) : ContractCustomException(message)
{
    public override string ToString()
    {
        return $"ContractException: {Message}";
    }
}

public class ContractCustomerNotFoundException(string message) : ContractCustomException(message)
{
    public override string ToString()
    {
        return $"ContractException: {Message}";
    }
}