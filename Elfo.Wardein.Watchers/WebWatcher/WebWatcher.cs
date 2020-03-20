using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.NotificationService;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcher : IWatcher
    {
        private readonly WebWatcherConfig configuration;
        private readonly IAmWatcherPersistenceService watcherPersistenceService;
        protected static ILogger log = LogManager.GetCurrentClassLogger();
        public string Name { get; }
        public string Group { get; }
        public const string DefaultName = "Web Watcher";

        protected WebWatcher(string name, WebWatcherConfig config,  string group)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration),
                    "Web Watcher configuration has not been provided.");
            }

            Name = name;
            this.configuration = config;
            Group = group;
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService(configuration.ConnectionString);
        }

        public async Task<IWatcherCheckResult> ExecuteAsync()
        {
            var baseUrl = configuration.Uri.ToString();

            var fullUrl = configuration.Request.GetFullUrl(baseUrl);
            try
            {
                var response = await httpService.ExecuteAsync(baseUrl, configuration.Request, configuration.Timeout);
                var isValid = HasValidResponse(response);
                if (!isValid)
                {
                    return WebWatcherCheckResult.Create(this, false,
                        configuration.Uri, configuration.Request, response,
                        $"Web endpoint: '{fullUrl}' has returned an invalid response with status code: {response.StatusCode}.");
                }

                return await EnsureAsync(fullUrl, response);
            }
            catch (TaskCanceledException)
            {
                return WebWatcherCheckResult.Create(this,
                    false, configuration.Uri,
                    configuration.Request, null,
                    $"A connection timeout occurred while trying to access the Web endpoint: '{fullUrl}'.");
            }
            catch (Exception exception)
            {
                throw new WatcherException($"There was an error while trying to access the Web endpoint: '{fullUrl}'.",
                    exception);
            }
        }

        private async Task<IWatcherCheckResult> EnsureAsync(string fullUrl, IHttpResponse response)
        {
            var isValid = true;
            if (configuration.EnsureThatAsync != null)
                isValid = await configuration.EnsureThatAsync?.Invoke(response);

            isValid = isValid && (configuration.EnsureThat?.Invoke(response) ?? true);

            return WebWatcherCheckResult.Create(this,
                isValid, configuration.Uri,
                configuration.Request, response,
                $"Web endpoint: '{fullUrl}' has returned a response with status code: {response.StatusCode}.");
        }

        private bool UseAuthentication(IHttpRequest request)
        {
            return false;
        }

        private List<string> AuthCredentials(string username, string password)
        {
            List<string> result = null;

            CredentialCache.DefaultNetworkCredentials.UserName = username;
            CredentialCache.DefaultNetworkCredentials.Password = password;

            using (var authtHandler = new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials })
            {
                using (var httpClient = new HttpClient(authtHandler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Keep-Alive"));
                    HttpResponseMessage message = httpClient.GetAsync("<service URI>").Result;
                    if (message.IsSuccessStatusCode)
                    {
                        var inter = message.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<List<string>>(inter.Result);
                    }
                }
            }
            return result;
        }

        private bool HasValidResponse(IHttpResponse response)
        {
            if (response.IsValid && Config.AssertWithStatusCode)
            {
                //we need to ensure that response status code is 200

            }
            else if (response.IsValid && !String.IsNullOrEmpty(Config.AssertWithRegex))
            {
                //we need also to understand if the regex matched the html response
            }
            return true;
        }

        internal virtual async Task RunCheck()
        {
            Log.Info($"{Environment.NewLine}> CHECKING SERVICES HEALTH");

            //if (Config.AssertWithStatusCode)
            //{
            //    //we need to ensure that response status code is 200
            //}
            //else if(String.IsNullOrEmpty(Config.AssertWithRegex))
            //{
            //    //we need also to understand if the regex matched the html response
            //}



            foreach (var service in Config.Services)
            {
                using (var persistenceService = ServicesContainer.PersistenceService(Const.DB_PATH))
                {
                    IAmResponceManager serviceManager = GetServiceManager();
                    if (serviceManager == null)
                        continue; // If the service doesn't exist, skip the check 

                    var notificationService = ServicesContainer.NotificationService(GetNotificationType());
                    var item = persistenceService.GetEntityById(service.ServiceName);

                    if (!serviceManager.IsStillAlive)
                    {
                        //try to restart 
                        await PerformActionOnServiceDown();
                    }
                    else
                    {
                        await PerformActionOnServiceAlive();
                    }

                    #region Local Functions

                    IAmResponceManager GetServiceManager()
                    {
                        IAmResponceManager svc = null;
                        try
                        {
                            svc = ServicesContainer.ServiceManager(service.ServiceName, service.ServiceManagerType);
                        }
                        catch (ArgumentNullException ex)
                        {
                            log.Warn(ex.Message);
                        }
                        return svc;
                    }

                    NotificationType GetNotificationType()
                    {
                        if (!Enum.TryParse<NotificationType>(service.NotificationType, out NotificationType result))
                            throw new ArgumentException($"Notification type {service.NotificationType} not supported");
                        return result;
                    }

                    async Task PerformActionOnServiceDown()
                    {
                        serviceManager.Restart();

                        if (IsRetryCountExceededOrEqual() && IsMultipleOfMaxRetryCount())
                        {
                            if (IAmAllowedToSendANewNotification())
                            {
                                log.Warn($"Sending Fail Notification");
                                await notificationService.SendNotificationAsync(service.RecipientAddress, service.FailMessage, $"Attention: {service.ServiceName} service is down");
                                item.LastNotificationSentAtThisTimeUTC = DateTime.UtcNow;
                            }
                        }
                        log.Info($"{service.ServiceName} is not active");

                        #region Local Functions

                        bool IAmAllowedToSendANewNotification()
                        {
                            return IsRepeatedMailTimeoutElapsed();

                            #region Local Functions

                            bool IsRepeatedMailTimeoutElapsed()
                            {
                                var timeout = GetServiceSendRepeatedNotificationAfterSecondsOrDefault();

                                return DateTime.UtcNow.Subtract(item.LastNotificationSentAtThisTimeUTC.GetValueOrDefault(DateTime.MinValue)) >= timeout;
                            }

                            #endregion
                        }

                        bool IsRetryCountExceededOrEqual() => item.RetryCount >= service.MaxRetryCount;

                        bool IsMultipleOfMaxRetryCount() => item.RetryCount % service.MaxRetryCount == 0;

                        #endregion
                    }

                    async Task PerformActionOnServiceAlive()
                    {
                        try
                        {
                            log.Info($"{service.ServiceName} is active");
                            if (item.RetryCount > 0)
                            {
                                log.Info($"Send Restored Notification");
                                await notificationService.SendNotificationAsync(service.RecipientAddress, service.RestoredMessage, $"Good news: {service.ServiceName} service has been restored succesfully");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, "Unable to send email");
                        }
                        finally
                        {
                            item.RetryCount = 0;
                        }
                    }

                    TimeSpan GetServiceSendRepeatedNotificationAfterSecondsOrDefault() =>
                        TimeSpan.FromSeconds(service.SendRepeatedNotificationAfterSeconds.GetValueOrDefault(Config.SendReminderEmailAfterRetryCount));


                    #endregion
                }
            }
        }
    }
}
