using System.Collections.Generic;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public enum HttpMethod
    {
        Get
    }

    public interface IHttpRequest
    {

        HttpMethod Method { get; }

        string Endpoint { get; }

        /// <summary>
        /// Request data that may be required for either POST or PUT request.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Request headers.
        /// </summary>
        IDictionary<string, string> Headers { get; }
    }


    /// <summary>
    /// Default implementation of the IHttpRequest.
    /// </summary>
    public class HttpRequest : IHttpRequest
    {
        public HttpMethod Method { get; } = HttpMethod.Get;
        public string Endpoint { get; }
        public object Data { get; }
        public IDictionary<string, string> Headers { get; }

        protected HttpRequest(HttpMethod method, string endpoint,
            IDictionary<string, string> headers = null, dynamic data = null)
        {

            Method = method;
            Endpoint = endpoint;
            Headers = headers ?? new Dictionary<string, string>();
            Data = data;
        }

        /// <summary>
        /// GET request with optional headers.
        /// </summary>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of IHttpRequest.</returns>
        public static IHttpRequest Get(IDictionary<string, string> headers = null)
            => new HttpRequest(HttpMethod.Get, string.Empty, headers);

        /// <summary>
        /// GET request with endpoint and optional headers.
        /// </summary>
        /// <param name="endpoint">Endpoint of the request</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of IHttpRequest.</returns>
        public static IHttpRequest Get(string endpoint, IDictionary<string, string> headers = null)
            => new HttpRequest(HttpMethod.Get, endpoint, headers);
    }
}
