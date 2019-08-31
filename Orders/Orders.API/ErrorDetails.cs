using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Orders
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class ErrorDetails
    {
        public string TraceId { get; }
        public int? StatusCode { get; }
        public string Message { get; }

        public ErrorDetails(string traceId, int statusCode, string message)
        {
            TraceId = traceId;
            StatusCode = statusCode;
            Message = message;
        }
    }

    public class ValidationErrorDetails : ErrorDetails
    {
        public IDictionary<string, IEnumerable<string>> Errors { get; }

        public ValidationErrorDetails(string traceId, IDictionary<string, IEnumerable<string>> errors) :
            base(traceId, StatusCodes.Status400BadRequest, "Input validation failed.")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}