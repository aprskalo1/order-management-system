namespace ContractService.Exceptions;

internal class ContractNotFoundException(string message) : ContractCustomException(message)
{
    public override string ToString()
    {
        return $"ContractException: {Message}";
    }
}