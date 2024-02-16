// License is 0BSD
// https://gist.github.com/djeikyb/b7d7b7afdc651396b90a5cb2279b58b9

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpSpy;

/// <summary>
/// Output format is modeled after httpie verbose format, which in turn is
/// modeled after the basic http text format that actually goes over the
/// wire in bytes. It's not in compliance tho, not least because there are
/// no carriage returns.
/// </summary>
public class HttpSpyHandler : DelegatingHandler
{
    private readonly ILogger<HttpSpyHandler> _logger;

    public HttpSpyHandler(ILogger<HttpSpyHandler> logger)
    {
        _logger = logger;
    }

    /// <param name="logger">Invoked twice. Once for request, again for response.</param>
    /// <param name="inner">Invoked to actually do the http work</param>
    public HttpSpyHandler(ILogger<HttpSpyHandler> logger, HttpMessageHandler inner) : base(inner)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct
    )
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            var s = await Build(request, ct);
            s.Insert(0, "🐡 request\n\n");
            _logger.LogTrace(s.ToString());
        }

        var rs = await base.SendAsync(request, ct);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            var s = await Build(rs, ct);
            s.Insert(0, "🍣 response\n\n");
            _logger.LogTrace(s.ToString());
        }

        return rs;
    }

    private static async Task<StringBuilder> Build(
        HttpRequestMessage request,
        CancellationToken ct
    )
    {
        var s = new StringBuilder();

        // line 1: POST /v1/login HTTP/1.1
        s.Append(request.Method);
        s.Append(" ").Append(request.RequestUri?.PathAndQuery);
        s.Append(" HTTP/").Append(request.Version);
        s.Append("\n");

        // line 2..n: User-Agent: HTTPie/2.5.0
        var headers = new List<(string Key, string? Value)>();
        foreach (var (k, values) in request.Headers.NonValidated)
        {
            foreach (var v in values)
            {
                headers.Add((k, v));
            }
        }

        if (request.Content != null)
        {
            foreach (var (k, values) in request.Content.Headers.NonValidated)
            {
                foreach (var v in values)
                {
                    headers.Add((k, v));
                }
            }
        }

        // this is really a connection detail..
        // listing here, as "host", is confusing af
        // because it looks like a duplicate host header
        // headers.Add(("Host", request.RequestUri?.Host));

        headers.Sort(
            (a, b) =>
            {
                var compareKey = string.CompareOrdinal(a.Key, b.Key);
                return compareKey == 0
                    ? string.CompareOrdinal(a.Value, b.Value)
                    : compareKey;
            }
        );

        foreach (var h in headers)
        {
            s.Append(h.Key).Append(": ").Append(h.Value).Append('\n');
        }

        // last: print content
        if (request.Content != null)
        {
            await request.Content.LoadIntoBufferAsync();
            s.Append('\n');
            var rqbody = await request.Content.ReadAsStringAsync(ct);
            s.Append(rqbody);
        }

        return s;
    }

    private static async Task<StringBuilder> Build(
        HttpResponseMessage msg,
        CancellationToken ct = default
    )
    {
        var s = new StringBuilder();

        // line 1: HTTP/1.1 200 OK
        s.Append("HTTP/").Append(msg.Version);
        s.Append(' ').Append((int)msg.StatusCode);
        s.Append(' ').Append(msg.ReasonPhrase);
        s.Append('\n');

        // line 2..n: User-Agent: HTTPie/2.5.0
        var headers = new List<(string Key, string? Value)>();
        foreach (var (k, values) in msg.Headers.NonValidated)
        {
            foreach (var v in values)
            {
                headers.Add((k, v));
            }
        }

        foreach (var (k, values) in msg.Content.Headers.NonValidated)
        {
            foreach (var v in values)
            {
                headers.Add((k, v));
            }
        }

        headers.Sort(
            (a, b) =>
            {
                var compareKey = string.CompareOrdinal(a.Key, b.Key);
                return compareKey == 0
                    ? string.CompareOrdinal(a.Value, b.Value)
                    : compareKey;
            }
        );

        foreach (var h in headers)
        {
            s.Append(h.Key).Append(": ").Append(h.Value).Append('\n');
        }

        // last: print content
        var notTooLong = msg.Content.Headers.ContentLength is null or < 1024 * 10;
        var isJson = msg.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json;
        if (notTooLong && isJson)
        {
            await msg.Content.LoadIntoBufferAsync();

            var body = await msg.Content.ReadAsStringAsync(ct);

            if (!string.IsNullOrEmpty(body))
            {
                s.Append('\n');
                s.Append(body);
            }
        }
        else
        {
            s.Append("\n");
            s.Append("⚠️ content suppressed 🥑");
        }

        return s;
    }
}
