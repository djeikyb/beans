using System;
using System.Text.Json.Serialization;

namespace BeanJuice.Model.Json.Gitlab;

public class Job
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("stage")]
    public string? Stage { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("commit")]
    public JobCommit? Commit { get; set; }

    [JsonPropertyName("pipeline")]
    public JobPipeline? Pipeline { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("started_at")]
    public DateTimeOffset? StartedAt { get; set; }

    [JsonPropertyName("finished_at")]
    public DateTimeOffset? FinishedAt { get; set; }

    [JsonPropertyName("duration")]
    public decimal? Duration { get; set; }
}
