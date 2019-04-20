using System.Collections.Generic;

namespace Clients.Common
{
    public class ErrorDetails
    {
        public string TraceId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class ValidationErrorDetails : ErrorDetails
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
    }
}