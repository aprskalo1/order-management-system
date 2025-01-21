using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrderService.Exceptions;

public class OrderExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not OrderCustomException orderCustomException) return;
        context.Result = context.Exception switch
        {
            OrderCreationException => new NotFoundObjectResult(orderCustomException.Message),
            _ => new BadRequestObjectResult(orderCustomException.Message)
        };

        context.ExceptionHandled = true;
    }
}