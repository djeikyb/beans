using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BeanJuice.Model.Json;
using BeanJuice.Model.Json.Gitlab;

namespace BeanJuice.Services;

// [Headers("Authorization: Bearer", "Accept: application/json")]
public interface IGitLabApi
{
    // implementation source is generated

    // [Get("/projects/{projectId}/jobs?pagination=keyset&per_page={perPage}&scope[]=success")]
    Task<IApiResponse<List<Job>>> ListProjectJobsSuccessful(
        int projectId,
        int perPage = 20,
        CancellationToken ct = default,
        GitlabPaginationParams? @params = null
    );
}

public class GitlabApi : IGitLabApi
{
    private readonly HttpClient _client;

    public GitlabApi(HttpClient client)
    {
        _client = client;
    }

    public Task<IApiResponse<List<Job>>> ListProjectJobsSuccessful(
        int projectId,
        int perPage = 20,
        CancellationToken ct = default,
        GitlabPaginationParams? @params = null
    )
    {
        var query = new QueryString();

        query["pagination"] = "keyset";
        query["per_page"] = perPage.ToString();
        query["scope[]"] = "success";

        if (@params?.IdAfter is { } idAfter) query["id_after"] = idAfter;
        if (@params?.Cursor is { } cursor) query["cursor"] = cursor;

        return Get<List<Job>>($"/api/v4/projects/{projectId}/jobs", query.ToString(), ct);
    }

    private async Task<IApiResponse<T>> Get<T>(string path, string? query = null, CancellationToken ct = default)
    {
        Guard.Against.Null(_client.BaseAddress);
        var rq = new HttpRequestMessage();
        rq.Method = HttpMethod.Get;
        rq.RequestUri = new UriBuilder(_client.BaseAddress) { Path = path, Query = query }.Uri;
        return await Send<T>(rq, ct).ConfigureAwait(false);
    }

    private async Task<IApiResponse<T>> Send<T>(HttpRequestMessage rq, CancellationToken ct = default)
    {
        ApiException? error = null;
        var obj = default(T);
        rq.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));

        var rs = await _client.SendAsync(rq, ct).ConfigureAwait(false);
        string? text = null;
        try
        {
            text = await rs.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            obj = (T?)JsonSerializer.Deserialize(text, typeof(T), GitlabJsonContext.Default);
        }
        catch (Exception e)
        {
            error = new ApiException(rq, rs, text, e);
        }

        return new ApiResponse<T>(rs, obj, error);
    }
}
