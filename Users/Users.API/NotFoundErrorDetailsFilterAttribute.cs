using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Users
{
    public class NotFoundErrorDetailsFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is NotFoundObjectResult result && result.Value is string message)
            {
                context.Result = new NotFoundObjectResult(
                    new ErrorDetails(
                        context.HttpContext.TraceIdentifier,
                        StatusCodes.Status404NotFound,
                        message)
                );
            }
        }
    }
}