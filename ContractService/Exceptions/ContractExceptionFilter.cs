using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractService.Exceptions;

public class ContractExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ContractCustomException contractCustomException) return;
        context.Result = context.Exception switch
        {
            ContractNotFoundException => new NotFoundObjectResult(contractCustomException.Message),
            _ => new BadRequestObjectResult(contractCustomException.Message)
        };

        context.ExceptionHandled = true;
    }
}