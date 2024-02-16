using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BeanJuice.Model.Services;

namespace BeanJuice.Services;

public class GitlabAuthHeaderHandler : DelegatingHandler
{
    private readonly IGitlabAuthTokenStore _gitlabAuthTokenStore;

    public GitlabAuthHeaderHandler(IGitlabAuthTokenStore gitlabAuthTokenStore)
    {
        this._gitlabAuthTokenStore = gitlabAuthTokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _gitlabAuthTokenStore.GetToken(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct).ConfigureAwait(false);
    }
}
