using Elfo.Wardein.Abstractions.PerformanceWatcher;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.WebWatcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Elfo.Wardein.Abstractions.Configuration.Models.PerformanceWatcherConfigurationModel;
using System.Diagnostics;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class HttpClientUrlPerformanceManager : IAmUrlPerformanceManager
    {
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task<bool> IsHealthy(PerformanceWatcherConfigurationModel configuration)
        {
            HttpResponseMessage response = null;
            var apiClient = InitializeApiClient(configuration);
            var apiCallExecutionTimeAccetable = false;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (configuration.Method == HttpCallApiMethod.Get)
                    response = await apiClient.GetAsync(configuration.Url);
                else
                    response = await apiClient.PostAsync(configuration.Url, new StringContent(JsonConvert.SerializeObject(configuration.Body ?? new Object()), UnicodeEncoding.UTF8, "application/json"));

                stopwatch.Stop();

                apiCallExecutionTimeAccetable = stopwatch.ElapsedMilliseconds < configuration.FirstThresholdInMilliseconds;
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error($"Exception got while waiting response from {configuration.Url.AbsoluteUri} - {ex}");
                throw;
            }
            var htmlResponse = await response.Content.ReadAsStringAsync();
            if (!configuration.AssertWithStatusCode)
            {
                if (!IsWebsiteAvailable(response))
                    return false;
                else if (!string.IsNullOrWhiteSpace(configuration.AssertWithRegex))
                {
                    return await CheckIsMatch(configuration.AssertWithRegex, htmlResponse);
                }
                else
                {
                    return apiCallExecutionTimeAccetable;
                }
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    log.Warn($"PerformanceWatcher on {configuration.UrlAlias} failed with status code: {response.StatusCode}");
                    return false;
                }
                else
                {
                    return await CheckIsMatch(configuration.AssertWithRegex, htmlResponse) ? apiCallExecutionTimeAccetable : false;
                }
            }
        }

        private async Task<bool> CheckIsMatch(string assertionRegex, string response)
        {
            if (!string.IsNullOrWhiteSpace(assertionRegex))
            {
                var isMatch = Regex.IsMatch(response, assertionRegex, RegexOptions.Singleline);
                if (isMatch)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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

        private HttpClient InitializeApiClient(PerformanceWatcherConfigurationModel configuration)
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

            if (configuration.Headers?.Count > 0)
                foreach (var header in configuration.Headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

            return client;
        }

        private bool IsWebsiteAvailable(HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.ServiceUnavailable;
        }
    }
}
