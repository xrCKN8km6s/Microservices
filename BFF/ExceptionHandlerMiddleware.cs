using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Clients.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProblemDetails = Clients.Common.ProblemDetails;
using ValidationProblemDetails = Clients.Common.ValidationProblemDetails;

namespace BFF
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly ApiBehaviorOptions _options;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IOptions<ApiBehaviorOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured");

                ProblemDetails error;

                switch (ex)
                {
                    case ApiException<ValidationProblemDetails> clientException when clientException.Result != null:
                        context.Response.StatusCode = clientException.StatusCode;
                        error = clientException.Result;
                        break;
                    case ApiException<ProblemDetails> clientException when clientException.Result != null:
                        context.Response.StatusCode = clientException.StatusCode;
                        error = clientException.Result;
                        break;
                    default:

                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        error = new ProblemDetails
                        {
                            Type = _options.ClientErrorMapping[StatusCodes.Status500InternalServerError].Link,
                            Title = _options.ClientErrorMapping[StatusCodes.Status500InternalServerError].Title,
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = ex.Message
                        };

                        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                        if (traceId != null)
                        {
                            error.Extensions["traceId"] = traceId;
                        }
                        break;
                }

                await context.WriteResultAsync(new ObjectResult(error));
            }
        }
    }
}
