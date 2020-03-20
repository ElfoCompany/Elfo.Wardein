using Elfo.Firmenich.Wardein.Abstractions.WebWatcher;
using Elfo.Wardein.Core.ServiceManager;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Core.ServiceManager
{
    public class HttpClientUrlResponseManager : IAmUrlResponseManager
    {

        public Task<bool> IsHealty(bool assertWithStatusCode, string assertWithRegex)
        {
            var endPoint = "";
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("http://localhost:55587/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Keep-Alive"));

                var response = client.GetAsync(endPoint);
                HttpResponseMessage httpWebResponse = response.Result;

                if (!assertWithStatusCode)
                {
                    if (!string.IsNullOrEmpty(assertWithRegex))
                    {
                        var isMatch = false;
                        //check if html content matches the regex
                        if (isMatch)
                        {
                            return Task.FromResult(false);
                        }
                        else
                        {
                            return Task.FromResult(true);
                        }
                    }
                    else
                    {
                        return Task.FromResult(true);
                    }
                }
                else
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        return Task.FromResult(false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(assertWithRegex))
                        {
                            var isMatch = false;
                            //check if html content matches the regex
                            if (isMatch)
                            {
                                return Task.FromResult(false);
                            }
                            else
                            {
                                return Task.FromResult(true);
                            }
                        }
                    }

                }
                return Task.FromResult(false);
            }
        }

        public void RestartPool(string poolName)
            => new IISPoolManager(poolName).Restart();
    }
}
