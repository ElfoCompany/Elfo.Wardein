using Elfo.Firmenich.Wardein.Abstractions.WebWatcher;
using Elfo.Wardein.Core.ServiceManager;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Core.ServiceManager
{
    public class HttpClientUrlResponseManager : IAmUrlResponseManager
    {

        public async Task<bool> IsHealthy(bool assertWithStatusCode, string assertWithRegex)
        {
            var endPoint = "";
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                // TODO support authentication
                client.BaseAddress = new Uri("http://google.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(endPoint);

                if (!assertWithStatusCode)
                {
                    if (!string.IsNullOrWhiteSpace(assertWithRegex))
                    {
                        return await CheckIsMatch(assertWithRegex, response);
                    }
                    else
                    {
                        return await Task.FromResult(true);
                    }
                }
                else
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return await Task.FromResult(false);
                    }
                    else
                    {
                        return await CheckIsMatch(assertWithRegex, response);
                    }
                }
            }
        }

        private async Task<bool> CheckIsMatch(string assertionRegex, HttpResponseMessage response)
        {
            if (!string.IsNullOrWhiteSpace(assertionRegex))
            {
                var htmlResponse = await response.Content.ReadAsStringAsync();
                var isMatch = Regex.IsMatch(htmlResponse, assertionRegex);
                if (isMatch)
                {
                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
            else
            {
                return await Task.FromResult(true);
            }
        }

        public Task RestartPool(string poolName)
        {
            new IISPoolManager(poolName).Restart();
            return Task.CompletedTask;
        }  
    }
}
