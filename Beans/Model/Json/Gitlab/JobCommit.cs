using System.Text.Json.Serialization;

namespace BeanJuice.Model.Json.Gitlab;

public class JobCommit
{
    [JsonPropertyName("short_id")]
    public string? ShortId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author_email")]
    public string? AuthorEmail { get; set; }
}
