using Elfo.Wardein.Abstractions.PerformanceWatcher;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class HttpClientUrlPerformanceManager : BaseHttpClientUrlManager, IAmUrlPerformanceManager
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
    }
}
