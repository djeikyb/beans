using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BeanJuice.Services;

public class QueryString
{
    private readonly Dictionary<string, string> _queryParameters = new();

    public string this[string key]
    {
        set => _queryParameters[key] = value;
    }

    public override string? ToString()
    {
        if (_queryParameters.Count == 0) return null;
        var pairs = _queryParameters.ToList();
        var sb = new StringBuilder();
        for (int i = 0; i < pairs.Count; i++)
        {
            if (i != 0) sb.Append('&');
            var kv = pairs[i];
            sb.Append(kv.Key).Append('=').Append(kv.Value);
        }

        return sb.ToString();
    }
}
