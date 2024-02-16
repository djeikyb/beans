// // https://gist.github.com/martincostello/53b10574fbee2d8bc7b0f75bf0855a84
//
// using System;
// using System.Linq;
// using System.Net.Http;
// using System.Net.Http.Json;
// using System.Reflection;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Threading;
// using System.Threading.Tasks;
// using Refit;
//
// namespace BeanJuice.Model.Json;
//
// /// <summary>
// /// A class representing an implementation of <see cref="IHttpContentSerializer"/> that can be
// /// used with the <c>System.Text.Json</c> source generators. This class cannot be inherited.
// /// </summary>
// public sealed class SystemTextJsonContentSerializerForSourceGenerator : IHttpContentSerializer
// {
//     //// Based on https://github.com/reactiveui/refit/blob/main/Refit/SystemTextJsonContentSerializer.cs
//
//     private readonly JsonSerializerContext _jsonSerializerContext;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="SystemTextJsonContentSerializerForSourceGenerator"/> class.
//     /// </summary>
//     /// <param name="jsonSerializerContext">The JSON serializer context to use.</param>
//     public SystemTextJsonContentSerializerForSourceGenerator(JsonSerializerContext jsonSerializerContext)
//     {
//         ArgumentNullException.ThrowIfNull(jsonSerializerContext);
//         _jsonSerializerContext = jsonSerializerContext;
//     }
//
//     /// <inheritdoc />
//     public HttpContent ToHttpContent<T>(T item)
//     {
//         ArgumentNullException.ThrowIfNull(item);
//
//         return JsonContent.Create(item, typeof(T), options: _jsonSerializerContext.Options);
//     }
//
//     /// <inheritdoc />
//     public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
//     {
//         ArgumentNullException.ThrowIfNull(content);
//
//         using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
//
//         object? result = await JsonSerializer.DeserializeAsync(
//                 stream,
//                 typeof(T),
//                 _jsonSerializerContext,
//                 cancellationToken
//             )
//             .ConfigureAwait(false);
//
//         return (T?)result;
//     }
//
//     /// <inheritdoc />
//     public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
//     {
//         ArgumentNullException.ThrowIfNull(propertyInfo);
//
//         return propertyInfo
//             .GetCustomAttributes<JsonPropertyNameAttribute>(true)
//             .Select(x => x.Name)
//             .FirstOrDefault();
//     }
// }
