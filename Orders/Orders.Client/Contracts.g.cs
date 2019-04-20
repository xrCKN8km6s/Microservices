﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.1.0.0 (NJsonSchema v9.13.28.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

namespace Orders.Client.Contracts
{
    #pragma warning disable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.1.0.0 (NJsonSchema v9.13.28.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface IOrdersClient
    {
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<OrderModel>> Orders_GetAsync();
    
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<OrderModel>> Orders_GetAsync(System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> Orders_CreateOrderAsync(string name);
    
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> Orders_CreateOrderAsync(string name, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> Orders_UpdateOrderStatusAsync(long orderId, int status);
    
        /// <exception cref="Clients.Common.ClientResponseException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> Orders_UpdateOrderStatusAsync(long orderId, int status, System.Threading.CancellationToken cancellationToken);
    
    }
    
    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.28.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class OrderModel 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("creationDateTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime CreationDateTime { get; set; }
    
        [Newtonsoft.Json.JsonProperty("orderStatus", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int OrderStatus { get; set; }
    
    
    }

    public partial class FileResponse : System.IDisposable
    {
        private System.IDisposable _client; 
        private System.IDisposable _response; 

        public int StatusCode { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public System.IO.Stream Stream { get; private set; }

        public bool IsPartial
        {
            get { return StatusCode == 206; }
        }

        public FileResponse(int statusCode, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.IO.Stream stream, System.IDisposable client, System.IDisposable response)
        {
            StatusCode = statusCode; 
            Headers = headers; 
            Stream = stream; 
            _client = client; 
            _response = response;
        }

        public void Dispose() 
        {
            if (Stream != null)
                Stream.Dispose();
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }
    }

    #pragma warning restore
}