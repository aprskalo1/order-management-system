namespace OrderService.Exceptions;

public abstract class OrderCustomException(string message) : Exception(message);