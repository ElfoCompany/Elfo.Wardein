using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.WebWatcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Elfo.Wardein.Abstractions.Configuration.Models.WebWatcherConfigurationModel;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class HttpClientUrlResponseManager : IAmUrlResponseManager
    {
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task<bool> IsHealthy(WebWatcherConfigurationModel configuration, HttpCallApiMethod method)
        {
            HttpResponseMessage response = null;
            var apiClient = InitializeApiClient(configuration);
            try
            {
                response = await apiClient.GetAsync(configuration.Url);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error($"Exception got while waiting response from {configuration.Url.AbsoluteUri} - {ex}");
                throw;
            }
            if (!configuration.AssertWithStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(configuration.AssertWithRegex))
                {
                    return await CheckIsMatch(configuration.AssertWithRegex, response);
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
                    return await CheckIsMatch(configuration.AssertWithRegex, response);
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

        public async Task RestartPool(string poolName)
        {
            await new IISPoolManager(poolName).Restart();
        }

        HttpClient InitializeApiClient(WebWatcherConfigurationModel configuration)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true, PreAuthenticate = true });
            client.BaseAddress = new Uri(configuration.Url.AbsoluteUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
