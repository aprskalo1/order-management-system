using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CustomerService.Exceptions;

public class CustomerExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not CustomerCustomException customerCustomException) return;
        context.Result = context.Exception switch
        {
            CustomerNotFoundException => new NotFoundObjectResult(customerCustomException.Message),
            _ => new BadRequestObjectResult(customerCustomException.Message)
        };

        context.ExceptionHandled = true;
    }
}