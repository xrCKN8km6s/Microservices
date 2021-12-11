using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Orders.API.Controllers;

[Route("/error")]
[ApiController]
public class ErrorController : ControllerBase
{
    [OpenApiIgnore]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return Problem(
            detail: context.Error.StackTrace,
            title: context.Error.Message);
    }
}
