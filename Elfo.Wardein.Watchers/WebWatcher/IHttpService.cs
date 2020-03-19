using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public interface IHttpService
    {
        Task<IHttpResponse> ExecuteAsync(string baseUrl, IHttpRequest request, TimeSpan? timeout = null);
    }


    public class HttpService : IHttpService
    {
        private readonly HttpClient client;

        public HttpService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IHttpResponse> ExecuteAsync(string baseUrl, IHttpRequest request, TimeSpan? timeout = null)
        {
            SetRequestHeaders(request.Headers);
            SetTimeout(timeout);
            var response = await GetHttpResponseAsync(baseUrl, request);
            var data = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
            var valid = response.IsSuccessStatusCode;
            var responseHeaders = GetResponseHeaders(response.Headers);

            return valid
                ? HttpResponse.Valid(response.StatusCode, response.ReasonPhrase, responseHeaders, data)
                : HttpResponse.Invalid(response.StatusCode, response.ReasonPhrase, responseHeaders, data);
        }

        private async Task<HttpResponseMessage> GetHttpResponseAsync(string baseUrl, IHttpRequest request)
        {
            var fullUrl = request.GetFullUrl(baseUrl);

            return await ExecuteHttpResponseAsync(fullUrl, request);
        }

        private async Task<HttpResponseMessage> ExecuteHttpResponseAsync(string fullUrl, IHttpRequest request)
        {
            var method = request.Method;
            switch (method)
            {
                case HttpMethod.Get:
                    return await client.GetAsync(fullUrl);
                default:
                    throw new ArgumentException($"Invalid HTTP method: {method}.", nameof(method));
            }
        }

        private void SetTimeout(TimeSpan? timeout)
        {
            if (timeout > TimeSpan.Zero)
                client.Timeout = timeout.Value;
        }

        private void SetRequestHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                var existingHeader = client.DefaultRequestHeaders
                    .FirstOrDefault(x => string.Equals(x.Key, header.Key, StringComparison.CurrentCultureIgnoreCase));
                if (existingHeader.Key != null)
                    client.DefaultRequestHeaders.Remove(existingHeader.Key);

                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        private IDictionary<string, string> GetResponseHeaders(HttpResponseHeaders headers)
            => headers?.ToDictionary(header => header.Key, header => header.Value?.FirstOrDefault()) ??
               new Dictionary<string, string>();
    }
}
