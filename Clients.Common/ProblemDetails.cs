using Newtonsoft.Json;

namespace Clients.Common;

//TODO:These should be removed when NSwag will support System.Text.Json
//https://github.com/RicoSuter/NSwag/issues/2243
public class ProblemDetails
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("status")]
    public int? Status { get; set; }

    [JsonProperty("detail")]
    public string Detail { get; set; }

    [JsonProperty("instance")]
    public string Instance { get; set; }

    //NOTE: Deserialized by NSwag using Json.NET, Serialized by ASP.NET Core (System.Text.Json)
    //Need both attributes
    [JsonExtensionData]
    [System.Text.Json.Serialization.JsonExtensionData]
    public IDictionary<string, object> Extensions { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
}

public class ValidationProblemDetails : ProblemDetails
{
    [JsonProperty("errors")]
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
}
