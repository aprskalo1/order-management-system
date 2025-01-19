using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PriceService.Exceptions;

public class ProductExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ProductCustomException productCustomException) return;
        context.Result = context.Exception switch
        {
            ProductNotFoundException => new NotFoundObjectResult(productCustomException.Message),
            PriceDateCorrelationException => new BadRequestObjectResult(productCustomException.Message),
            _ => new BadRequestObjectResult(productCustomException.Message)
        };

        context.ExceptionHandled = true;
    }
}