using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace REAL_STATE.Utilities;

public sealed class SafeExceptionFilter(ILogger<SafeExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "Unhandled request error. Trace: {TraceId}", context.HttpContext.TraceIdentifier);
        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = 500, Title = "The operation could not be completed.",
            Extensions = { ["traceId"] = context.HttpContext.TraceIdentifier }
        }) { StatusCode = 500 };
        context.ExceptionHandled = true;
    }
}
