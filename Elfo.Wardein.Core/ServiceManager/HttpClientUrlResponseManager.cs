using Elfo.Wardein.Abstractions.WebWatcher;
using Elfo.Wardein.Core.ServiceManager;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class HttpClientUrlResponseManager : IAmUrlResponseManager
    {

        public async Task<bool> IsHealthy(bool assertWithStatusCode, string assertWithRegex, Uri url)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                // TODO support authentication
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(url);

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

        //private List<string> AuthCredentials(string username, string password)
        //{
        //    List<string> result = null;

        //    CredentialCache.DefaultNetworkCredentials.UserName = username;
        //    CredentialCache.DefaultNetworkCredentials.Password = password;

        //    using (var authtHandler = new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials })
        //    {
        //        using (var httpClient = new HttpClient(authtHandler))
        //        {
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Keep-Alive"));
        //            HttpResponseMessage message = httpClient.GetAsync("<service URI>").Result;
        //            if (message.IsSuccessStatusCode)
        //            {
        //                var inter = message.Content.ReadAsStringAsync();
        //                result = JsonConvert.DeserializeObject<List<string>>(inter.Result);
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}
