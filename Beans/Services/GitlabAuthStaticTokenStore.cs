using System.Threading;
using System.Threading.Tasks;
using BeanJuice.Model.Services;

namespace BeanJuice.Services;

public class GitlabAuthStaticTokenStore(string token) : IGitlabAuthTokenStore
{
    public Task<string> GetToken(CancellationToken? ct = default) => Task.FromResult(token);
    public void SetToken(string value) => token = value;
}
