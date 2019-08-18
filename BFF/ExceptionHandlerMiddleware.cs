using System;
using System.Threading.Tasks;
using Clients.Common;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BFF
{
    //TODO: logging/error handling, improve
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ErrorDetails error;

                switch (ex)
                {
                    case ClientException<ValidationErrorDetails> clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error = clientException.Result;
                        break;
                    case ClientException<ErrorDetails> clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error = clientException.Result;
                        break;
                    case ClientException clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error = new ErrorDetails
                        {
                            StatusCode = clientException.StatusCode,
                            TraceId = context.TraceIdentifier,
                            Message = clientException.Response
                        };
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        error = new ErrorDetails
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            TraceId = context.TraceIdentifier,
                            Message = ex.Message
                        };
                        break;
                }

                await context.WriteResultAsync(new ObjectResult(error) {ContentTypes = {"application/problem+json"}});
            }
        }
    }
}