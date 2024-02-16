using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BeanJuice.Model.Json.Gitlab;
using BeanJuice.Model.Services;

namespace BeanJuice.Services;

public class GitlabService : IGitlabService
{
    private readonly IGitLabApi _gitLabApi;

    public GitlabService(IGitLabApi gitLabApi)
    {
        _gitLabApi = gitLabApi;
    }

    public async IAsyncEnumerable<Job> ListLiveDeployJobs(
        int projectId,
        int maxPages,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        var foundJobs = ListProjectJobsSuccessful(projectId, maxPages, ct);
        await foreach (var job in foundJobs)
        {
            if (ct.IsCancellationRequested) break;
            if (job.Name != null && !job.Name.StartsWith("deploy live")) continue;
            yield return job;
        }
    }

    public async IAsyncEnumerable<Job> ListProjectJobsSuccessful(
        int projectId,
        int maxPages,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        GitlabPaginationParams? nextQueryParams = null;
        for (var pageIndex = 0; pageIndex < maxPages; pageIndex++)
        {
            using var rs = await _gitLabApi.ListProjectJobsSuccessful(projectId, perPage: 100, ct, nextQueryParams);
            if (rs.Error != null) throw rs.Error;
            if (rs.Content == null) yield break;

            foreach (var job in rs.Content) yield return job;

            nextQueryParams = GitlabLinkHeaderItem
                .FromHeader(rs.Headers)
                .Where(link => link.IsNext())
                .Select(link => link.ToPageParams())
                .FirstOrDefault();
            if (nextQueryParams == null) yield break;
        }
    }

    // public async IAsyncEnumerable<Job> ListProjectJobsSuccessful(
    //     int projectId,
    //     int maxPages,
    //     [EnumeratorCancellation] CancellationToken ct = default
    // )
    // {
    //     GitlabPaginationParams? qp;
    //     using (var rs = await _gitLabApi.ListProjectJobsSuccessful(projectId, ct: ct))
    //     {
    //         if (rs.Error != null) throw rs.Error;
    //         if (rs.Content == null) yield break; // yield break exits the entire function
    //         foreach (var job in rs.Content) yield return job;
    //
    //         qp = GitlabLinkHeaderItem
    //             .FromHeader(rs.Headers)
    //             .Where(x => x.IsNext())
    //             .Select(x => x.ToParams())
    //             .FirstOrDefault();
    //         if (qp == null) yield break;
    //     }
    //
    //     // start with 1 because we've already done the first page
    //     for (int pageIndex = 1; pageIndex < maxPages; pageIndex++)
    //     {
    //         (var jobs, qp) = await NextPage(projectId, qp, ct);
    //         foreach (var job in jobs) yield return job;
    //         if (qp == null) yield break;
    //     }
    // }

    // private async IAsyncEnumerable<Job> LowerCalculatedComplexity(
    //     int projectId,
    //     int maxPages,
    //     [EnumeratorCancellation] CancellationToken ct = default
    // )
    // {
    //     GitlabPaginationParams? qp = null;
    //     for (var pageIndex = 0; pageIndex < maxPages; pageIndex++)
    //     {
    //         (var jobs, qp) = await NextPage(projectId, qp, ct);
    //         foreach (var job in jobs) yield return job;
    //         if (qp == null) yield break;
    //     }
    // }
    //
    // private async Task<(IEnumerable<Job> jobs, GitlabPaginationParams? nextQueryParams)> NextPage(
    //     int projectId,
    //     GitlabPaginationParams? qp,
    //     CancellationToken ct = default
    // )
    // {
    //     using var rs = await _gitLabApi.ListProjectJobsSuccessful(projectId, perPage: 100,ct, @params: qp);
    //     if (rs.Error != null) throw rs.Error;
    //     if (rs.Content == null) return ([], null);
    //     var nextQueryParams = GitlabLinkHeaderItem
    //         .FromHeader(rs.Headers)
    //         .Where(x => x.IsNext())
    //         .Select(x => x.ToPageParams())
    //         .FirstOrDefault();
    //     return (rs.Content, nextQueryParams);
    // }
}
