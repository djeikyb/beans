using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;

namespace BeanJuice.Services;

public static class Extensions
{
    public static bool TryGet(this NameValueCollection nvc, string key, [NotNullWhen(true)] out string? value)
    {
        value = nvc.Get(key);
        return value != null;
    }

    public static bool TryGetFirstValue(this HttpHeaders headers, string name, [NotNullWhen(true)] out string? value)
    {
        if (headers.TryGetValues(name, out var values))
        {
            value = values.FirstOrDefault();
            return value is not null;
        }

        value = null;
        return false;
    }
}
