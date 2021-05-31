using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BFF
{
    // https://kristian.hellang.com/using-mvc-result-executors-in-middleware/
    // https://github.com/aspnet/Mvc/issues/7238
    public static class HttpContextExtensions
    {
        private static readonly RouteData EmptyRouteData = new();

        private static readonly ActionDescriptor EmptyActionDescriptor = new();

        public static Task WriteResultAsync<TResult>(this HttpContext context, TResult result)
            where TResult : IActionResult
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var executor = context.RequestServices.GetService<IActionResultExecutor<TResult>>() ??
                           throw new InvalidOperationException(
                               $"No result executor for '{typeof(TResult).FullName}' has been registered.");

            var routeData = context.GetRouteData() ?? EmptyRouteData;

            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            return executor.ExecuteAsync(actionContext, result);
        }
    }
}
