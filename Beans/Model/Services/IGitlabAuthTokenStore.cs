using System.Threading;
using System.Threading.Tasks;

namespace BeanJuice.Model.Services;

public interface IGitlabAuthTokenStore
{
    /// This is only an awaitable cause I was following an example that
    /// was thinking about refreshing a jwt.
    Task<string> GetToken(CancellationToken? ct);

    /// Arguably it's quite silly for this interface to have
    /// async Get => Task
    /// Set => void
    void SetToken(string value);
}
