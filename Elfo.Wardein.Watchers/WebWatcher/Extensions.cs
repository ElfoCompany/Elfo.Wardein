using System;
using System.Collections.Generic;
using System.Text;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public static class Extensions
    {
        //internal static string GetFullUrl(this IHttpRequest request, string baseUrl)
        //{
        //    var endpoint = request.Endpoint;
        //    if (string.IsNullOrWhiteSpace(endpoint))
        //        return baseUrl;

        //    if (baseUrl.EndsWith("/"))
        //        return $"{baseUrl}{(endpoint.StartsWith("/") ? endpoint.Substring(1) : $"{endpoint}")}";

        //    return $"{baseUrl}{(endpoint.StartsWith("/") ? endpoint : $"/{endpoint}")}";
        //}
    }
}
