using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeanJuice.Model.Json.Gitlab;

[JsonSerializable(typeof(List<Job>))]
// [JsonSerializable(typeof(Job))]
// [JsonSerializable(typeof(JobCommit))]
// [JsonSerializable(typeof(JobPipeline))]
public partial class GitlabJsonContext : JsonSerializerContext
{
    public static readonly GitlabJsonContext s_instance = new(
        new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            IncludeFields = false,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        }
    );
}
