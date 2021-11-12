using Elfo.Wardein.Abstractions.Configuration.Models.WatcherModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.ServiceManager
{
	public abstract class BaseHttpClientUrlManager
	{
        protected virtual async Task<bool> CheckIsMatch(string assertionRegex, string response)
        {
            if (!string.IsNullOrWhiteSpace(assertionRegex))
            {
                return !Regex.IsMatch(response, assertionRegex, RegexOptions.Singleline);
            }
            else
            {
                return true;
            }
        }

        public async Task RestartPool(string poolName)
        {
            await new IISPoolManager(poolName).Restart();
        }

        protected virtual bool IsWebsiteAvailable(HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.ServiceUnavailable;
        }

        protected virtual HttpClient InitializeApiClient(BaseUrlWatcherConfigurationModel configuration)
        {
            var handler = new HttpClientHandler
            {
                Credentials = new CredentialCache { { configuration.Url, "NTLM", CredentialCache.DefaultNetworkCredentials } },
                PreAuthenticate = true,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(configuration.Url.AbsoluteUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (configuration.Headers != null)
            {
                var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(configuration.Headers?.ToString());
                if (headers?.Count > 0)
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return client;
        }
    }
}
