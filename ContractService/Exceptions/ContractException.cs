namespace ContractService.Exceptions;

internal class ContractNotFoundException(string message) : ContractCustomException(message)
{
    public override string ToString()
    {
        return $"ContractException: {Message}";
    }
}
internal class ContractCustomerNotFoundException(string message) : ContractCustomException(message)
{
    public override string ToString()
    {
        return $"ContractException: {Message}";
    }
}