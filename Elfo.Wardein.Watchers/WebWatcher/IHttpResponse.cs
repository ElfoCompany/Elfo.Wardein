using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; }

        bool IsValid { get; }

        string ReasonPhrase { get; }

        IDictionary<string, string> Headers { get; }

        string Data { get; }
    }


    public class HttpResponse : IHttpResponse
    {
        public HttpStatusCode StatusCode { get; }
        public bool IsValid { get; }
        public string ReasonPhrase { get; }
        public IDictionary<string, string> Headers { get; }
        public string Data { get; }

        protected HttpResponse(HttpStatusCode statusCode, bool isValid,
            string reasonPhrase, IDictionary<string, string> headers, string data)
        {
            StatusCode = statusCode;
            IsValid = isValid;
            ReasonPhrase = reasonPhrase;
            Headers = headers ?? new Dictionary<string, string>();
            Data = data;
        }

        /// <summary>
        /// Valid HTTP response.
        /// </summary>
        /// <param name="statusCode">Status code of response.</param>
        /// <param name="reasonPhrase">Reason phrase.</param>
        /// <param name="headers">Response headers.</param>
        /// <param name="data">Response content.</param>
        /// <returns>Instance of IHttpResponse.</returns>
        public static IHttpResponse Valid(HttpStatusCode statusCode, string reasonPhrase,
            IDictionary<string, string> headers, string data) => new HttpResponse(statusCode, true,
                reasonPhrase, headers, data);

        /// <summary>
        /// Invalid HTTP response.
        /// </summary>
        /// <param name="statusCode">Status code of response.</param>
        /// <param name="reasonPhrase">Reason phrase.</param>
        /// <param name="headers">Response headers.</param>
        /// <param name="data">Response content.</param>
        /// <returns>Instance of IHttpResponse.</returns>
        public static IHttpResponse Invalid(HttpStatusCode statusCode, string reasonPhrase,
            IDictionary<string, string> headers, string data) => new HttpResponse(statusCode, false,
                reasonPhrase, headers, data);
    }
}
