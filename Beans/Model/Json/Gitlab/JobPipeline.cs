using System.Text.Json.Serialization;

namespace BeanJuice.Model.Json.Gitlab;

public class JobPipeline
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
