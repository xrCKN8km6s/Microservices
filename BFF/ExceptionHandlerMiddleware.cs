using System;
using System.Threading.Tasks;
using Common.ExceptionHandling;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//unfortunately ProblemDetails is a part of Mvc package, copied to local project
using ProblemDetails = Common.ExceptionHandling.ProblemDetails;


namespace BFF
{
    //TODO: logging/error handling, improve
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            _next = next;
            _env = env;
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
                var error = new ErrorDetails
                {
                    TraceId = context.TraceIdentifier
                };

                if (_env.IsDevelopment())
                {
                    error.Exception = ex;
                }

                switch (ex)
                {
                    case ClientResponseException<string> clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error.Message = clientException.Result;
                        break;
                    case ClientResponseException<ProblemDetails> clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error.Message = clientException.Result;
                        break;
                    case ClientResponseException clientException:
                        context.Response.StatusCode = clientException.StatusCode;
                        error.Message = clientException.Response;
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        error.Message = ex.Message;
                        break;
                }

                await context.WriteResultAsync(new ObjectResult(error));
            }
        }
    }
}