using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Firmenich.Wardein.Abstractions.WebWatcher;
using Elfo.Firmenich.Wardein.Core.ServiceManager;
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
    public class WebWatcher : WardeinWatcher<WebWatcherConfig>
    {
        private readonly WebWatcherConfig configuration;
        private readonly IAmWatcherPersistenceService watcherPersistenceService;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        protected WebWatcher(string name, WebWatcherConfig config, string group) : base(name, config, group)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration),
                    "Web Watcher configuration has not been provided.");
            }

            configuration = config;
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService(configuration.ConnectionString);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            Log.Info($"---\tStarting {Name}\t---");
            try
            {
                var guid = Guid.NewGuid();
                log.Info($"{Environment.NewLine}{"-".Repeat(24)} Services health check @ {guid} started {"-".Repeat(24)}");

                await RunCheck();

                log.Info($"{Environment.NewLine}{"-".Repeat(24)} Services health check @ {guid} finished {"-".Repeat(24)}{Environment.NewLine.Repeat(24)}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside polling action: {ex.ToString()}\n");
            }

            return await Task.FromResult<IWatcherCheckResult>(null);
        }

        internal virtual async Task RunCheck()
        {
            Log.Info($"{Environment.NewLine}> CHECKING WEB WATCHER HELTH");

          
                using (var persistenceService = ServicesContainer.PersistenceService(Const.DB_PATH))
                {

                    IAmUrlResponseManager urlResponseManager = new HttpClientUrlResponseManager();
                  
                    var notificationService = ServicesContainer.NotificationService(NotificationType.Mail);
                   // var item = persistenceService.GetEntityById(service.ServiceName);

                    var svc = watcherPersistenceService.UpsertCurrentStatus(1, 2, "SRVWEB07", false);

                    if (!await urlResponseManager.IsHealthy(configuration.AssertWithStatusCode, configuration.AssertWithRegex))
                    {

                        await PerformActionOnServiceDown(configuration.AssociatedIISPool);
                    }
                    else
                    {
                        await PerformActionOnServiceAlive();
                    }


                #region Local Functions

                //NotificationType GetNotificationType()
                //{
                //    if (!Enum.TryParse<NotificationType>(service.NotificationType, out NotificationType result))
                //        throw new ArgumentException($"Notification type {service.NotificationType} not supported");
                //    return result;
                //}

                async Task PerformActionOnServiceDown(string poolName)
                    {
                        if (IsRetryCountExceededOrEqual() && IsMultipleOfMaxRetryCount())
                        {
                                log.Warn($"Sending Fail Notification");
                                await notificationService.SendNotificationAsync(configuration.RecipientAddresses, configuration.FailureMessage, 
                                        $"Attention: {nameof(WebWatcher)} is down");
                        }
                        else if (poolName == string.Empty)
                        {
                            await urlResponseManager.RestartPool(poolName);
                            log.Info($"{nameof(WebWatcher)} was restarted");
                        }

                        #region Local Functions

                        bool IsRetryCountExceededOrEqual() => svc.Result.FailureCount == configuration.MaxRetryCount;

                        bool IsMultipleOfMaxRetryCount() => svc.Result.FailureCount % configuration.SendReminderEmailAfterRetryCount == 0;

                        #endregion
                    }

                    async Task PerformActionOnServiceAlive()
                    {
                        try
                        {
                            log.Info($"{nameof(WebWatcher)} is active");
                            if (svc.Result.PreviousStatus)
                            {
                                {
                                    log.Info($"Send Restored Notification");
                                    await notificationService.SendNotificationAsync(configuration.RecipientAddresses, configuration.RestoredMessage, 
                                          $"Good news: {nameof(WebWatcher)} has been restored succesfully");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, "Unable to send email");
                        }
                        finally
                        {
                            svc.Result.PreviousStatus = false;
                        }
                    }
                    #endregion
                }
        }

        public async Task<IWatcherCheckResult> ExecuteAsync()
        {
            //var baseUrl = configuration.Uri.ToString();

            //var fullUrl = configuration.Request.GetFullUrl(baseUrl);
            //try
            //{
            //    var response = await httpService.ExecuteAsync(baseUrl, configuration.Request, configuration.Timeout);

            //    if (!isValid)
            //    {
            //        return WebWatcherCheckResult.Create(this, false,
            //            configuration.Uri, configuration.Request, response,
            //            $"Web endpoint: '{fullUrl}' has returned an invalid response with status code: {response.StatusCode}.");
            //    }

            //    return await EnsureAsync(fullUrl, response);
            //}
            //catch (TaskCanceledException)
            //{
            //    return WebWatcherCheckResult.Create(this,
            //        false, configuration.Uri,
            //        configuration.Request, null,
            //        $"A connection timeout occurred while trying to access the Web endpoint: '{fullUrl}'.");
            //}
            //catch (Exception exception)
            //{
            //    throw new WatcherException($"There was an error while trying to access the Web endpoint: '{fullUrl}'.",
            //        exception);
            //}
            return Task.CompletedTask as IWatcherCheckResult;
        }

        private async Task<IWatcherCheckResult> EnsureAsync()
        {
            //var isValid = true;
            //if (configuration.EnsureThatAsync != null)
            //    isValid = await configuration.EnsureThatAsync?.Invoke(response);

            //isValid = isValid && (configuration.EnsureThat?.Invoke(response) ?? true);

            //return WebWatcherCheckResult.Create(this,
            //    isValid, configuration.Uri,
            //    configuration.Request, response,
            //    $"Web endpoint: '{fullUrl}' has returned a response with status code: {response.StatusCode}.");
            return Task.CompletedTask as IWatcherCheckResult;
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
    }
}
