using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using BeanJuice.Model.Json.Gitlab;

namespace BeanJuice.Model.Services;

public interface IGitlabService
{
    IAsyncEnumerable<Job> ListProjectJobsSuccessful(
        int projectId,
        int maxPages,
        CancellationToken ct = default
    );

    IAsyncEnumerable<Job> ListLiveDeployJobs(
        int projectId,
        int maxPages,
        CancellationToken ct = default
    );
}
