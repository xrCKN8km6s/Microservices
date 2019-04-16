using System;
using System.Collections.Generic;

namespace Common.ExceptionHandling
{
    public class ClientResponseException : Exception
    {
        public int StatusCode { get; }

        public string Response { get; }

        public Dictionary<string, IEnumerable<string>> Headers { get; }

        public ClientResponseException(string message, int statusCode, string response,
            Dictionary<string, IEnumerable<string>> headers, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return $"HTTP Response: \n\n{Response}\n\n{base.ToString()}";
        }
    }

    public class ClientResponseException<TResult> : ClientResponseException
    {
        public TResult Result { get; }

        public ClientResponseException(string message, int statusCode, string response,
            Dictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }
}