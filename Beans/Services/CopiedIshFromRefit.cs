using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BeanJuice.Services;

public class ApiException : Exception
{
    public ApiException(HttpRequestMessage rq, HttpResponseMessage rs, string? responseContent, Exception inner) : base(
        $"{rs.StatusCode} {rs.ReasonPhrase}",
        inner
    )
    {
    }
}

/// <summary>
/// Base interface used to represent an API response.
/// </summary>
public interface IApiResponse : IDisposable
{
    /// <summary>
    /// HTTP response headers.
    /// </summary>
    HttpResponseHeaders Headers { get; }

    /// <summary>
    /// HTTP response content headers as defined in RFC 2616.
    /// </summary>
    HttpContentHeaders? ContentHeaders { get; }

    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ContentHeaders))]
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccessStatusCode { get; }

    /// <summary>
    /// HTTP response status code.
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    /// The reason phrase which typically is sent by the server together with the status code.
    /// </summary>
    string? ReasonPhrase { get; }

    /// <summary>
    /// The HTTP Request message which led to this response.
    /// </summary>
    HttpRequestMessage? RequestMessage { get; }

    /// <summary>
    /// HTTP Message version.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// The <see cref="ApiException"/> object in case of unsuccessful response.
    /// </summary>
    ApiException? Error { get; }
}

/// <inheritdoc/>
public interface IApiResponse<out T> : IApiResponse
{
    /// <summary>
    /// Deserialized request content as <typeparamref name="T"/>.
    /// </summary>
    T? Content { get; }
}

public class ApiResponse<T> : IApiResponse<T>
{
    public ApiResponse(HttpResponseMessage rs, T? o, ApiException? error)
    {
        Headers = rs.Headers;
        ContentHeaders = rs.Content.Headers;
        IsSuccessStatusCode = rs.IsSuccessStatusCode;
        StatusCode = rs.StatusCode;
        ReasonPhrase = rs.ReasonPhrase;
        RequestMessage = rs.RequestMessage;
        Version = rs.Version;
        Error = error;
        Content = o;
    }

    public void Dispose()
    {
        RequestMessage?.Dispose();
    }


    public HttpResponseHeaders Headers { get; init; }
    public HttpContentHeaders? ContentHeaders { get; init; }
    public bool IsSuccessStatusCode { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public string? ReasonPhrase { get; init; }
    public HttpRequestMessage? RequestMessage { get; init; }
    public Version Version { get; init; }
    public ApiException? Error { get; init; }
    public T? Content { get; init; }
}
