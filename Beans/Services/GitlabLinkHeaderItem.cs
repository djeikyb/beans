using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;

namespace BeanJuice.Services;

public class GitlabPaginationParams
{
    // [AliasAs("id_after")]
    public string? IdAfter { get; set; }

    // [AliasAs("cursor")]
    public string? Cursor { get; set; }
}

internal partial class GitlabLinkHeaderItem
{
    public GitlabLinkHeaderItem(Uri uri, NameValueCollection @params)
    {
        Uri = uri;
        Params = @params;
    }

    public Uri Uri { get; private set; }
    public NameValueCollection Params { get; private set; }
    public bool IsNext() => Params.TryGet("rel", out var rel) && "next".Equals(rel);

    public GitlabPaginationParams? ToPageParams()
    {
        var qp = new GitlabPaginationParams();
        var nvc = HttpUtility.ParseQueryString(Uri.Query);

        if (nvc.TryGet("id_after", out var idAfter))
        {
            qp.IdAfter = idAfter;
            return qp;
        }

        if (nvc.TryGet("cursor", out var cursor))
        {
            qp.Cursor = cursor;
            return qp;
        }

        return null;
    }

    public static List<GitlabLinkHeaderItem> FromHeader(HttpResponseHeaders headers)
    {
        if (!headers.TryGetFirstValue("Link", out var linkHeader)) return [];
        var links = new List<GitlabLinkHeaderItem>();
        var m = HttpHeaderLinkRegex().Match(linkHeader);
        var urls = m.Groups[1];
        var rels = m.Groups[2];
        for (int i = 0; i < urls.Captures.Count; i++)
        {
            var u = urls.Captures[i].Value;
            var r = rels.Captures[i].Value;
            var nvc = new NameValueCollection();
            nvc["rel"] = r;
            links.Add(new GitlabLinkHeaderItem(new Uri(u), nvc));
        }

        return links;
    }

    public static List<GitlabLinkHeaderItem> FromHeader(string linkHeaderValue)
    {
        var links = new List<GitlabLinkHeaderItem>();
        var m = HttpHeaderLinkRegex().Match(linkHeaderValue);
        var urls = m.Groups[1];
        var rels = m.Groups[2];
        for (int i = 0; i < urls.Captures.Count; i++)
        {
            var u = urls.Captures[i].Value;
            var r = rels.Captures[i].Value;
            var nvc = new NameValueCollection();
            nvc["rel"] = r;
            links.Add(new GitlabLinkHeaderItem(new Uri(u), nvc));
        }

        return links;
    }

    /// <summary>
    /// Group 1 will be urls
    /// Group 2 will the first rel attribute
    /// </summary>
    [GeneratedRegex("<(.*?)>; rel=\"(.*?)\"")]
    private static partial Regex HttpHeaderLinkRegex();
}
