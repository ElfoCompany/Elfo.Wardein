using Elfo.Wardein.Abstractions;
using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.HeartBeat;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Abstractions.WebWatcher;
using Elfo.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Core.HeartBeat;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.NotificationService;
using Elfo.Wardein.Core.Persistence;
using Elfo.Wardein.Core.ServiceManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Elfo.Wardein.Core
{
    public class ServicesContainer
    {
        #region Local Variables

        private static object sync = new object();
        private IServiceCollection serviceCollection;
        private static volatile ServicesContainer serviceContainer;
        private ServiceProvider serviceProvider;
        private readonly WardeinBaseConfiguration wardeinBaseConfiguration;

        #endregion

        #region Constructor

        protected ServicesContainer(WardeinBaseConfiguration wardeinBaseConfiguration)
        {
            Configure();
            this.wardeinBaseConfiguration = wardeinBaseConfiguration;
        }

        #endregion

        #region Configurations

        protected void Configure()
        {

            serviceCollection = new ServiceCollection()
                .AddSingleton<IAmMailConfigurationManager>(sp =>
                {
                    switch (wardeinBaseConfiguration.MailConnectionType)
                    {
                        case ConnectionType.FileSystem:
                            return new MailConfigurationManagerFromJSON(wardeinBaseConfiguration.MailConnectionString);
                        default:
                            throw new NotImplementedException();

                    }
                })
                .AddSingleton<IAmWardeinConfigurationManager>(sp =>
                {
                    switch (wardeinBaseConfiguration.StorageConnectionType)
                    {
                        case ConnectionType.FileSystem:
                            return new WardeinConfigurationManagerFromJSON(wardeinBaseConfiguration.StorageConnectionString);
                        case ConnectionType.Oracle:
                            return new OracleWardeinConfigurationManager(sp.GetService<IOracleHelper>(), HostHelper.GetName());
                        default:
                            throw new NotImplementedException();

                    }
                })
                .AddSingleton<OracleConnectionConfiguration>(sp =>
                {
                    var configs = wardeinBaseConfiguration.OracleAdditionalParams;
                    var builder = new OracleConnectionConfiguration.Builder(wardeinBaseConfiguration.StorageConnectionString);
                    if (configs != null)
                        builder
                            .WithClientId(configs.ClientId)
                            .WithClientInfo(configs.ClientInfo)
                            .WithModuleName(configs.ModuleName)
                            .WithDateLanguage(configs.DateLanguage);
                    return builder.Build();
                })
                .AddSingleton<IAmUrlResponseManager>(new HttpClientUrlResponseManager())
                .AddTransient<IOracleHelper>(sp => new OracleHelper(sp.GetService<OracleConnectionConfiguration>()))
                .AddTransient<IAmWatcherPersistenceService>(sp =>
                {
                    switch (wardeinBaseConfiguration.StorageConnectionType)
                    {
                        case ConnectionType.Oracle:
                            return new OracleWatcherPersistenceService(sp.GetService<OracleConnectionConfiguration>());
                        default:
                            throw new NotImplementedException();

                    }
                })
                .AddTransient<Func<NotificationType, IAmNotificationService>>(sp => notificationType =>
                {
                    switch (notificationType)
                    {
                        case NotificationType.Mail:
                            return new MailNotificationService();
                        case NotificationType.Teams:
                            return new TeamsNotificationService();
                        default:
                            throw new KeyNotFoundException($"Notification service {notificationType.ToString()} not supported yet");
                    }
                })
                .AddTransient<Func<ServiceManagerType, string, IAmServiceManager>>(sp => (serviceManagerType, serviceName) =>
                {
                    switch (serviceManagerType)
                    {
                        case ServiceManagerType.WindowsService:
                            return new WindowsServiceManager(serviceName);
                        case ServiceManagerType.IISPool:
                            return new IISPoolManager(serviceName);
                        default:
                            throw new KeyNotFoundException($"Notification service {serviceManagerType.ToString()} not supported yet");
                    }
                })
                .AddTransient<IAmWardeinHeartBeatPersistanceService>(sp =>
                {
                    switch (wardeinBaseConfiguration.StorageConnectionType)
                    {
                        case ConnectionType.Oracle:
                            return new OracleWardeinHeartBeatPersistanceService(sp.GetService<OracleConnectionConfiguration>());
                        default:
                            throw new NotImplementedException();

                    }
                });


            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the istance as singleton
        /// </summary>
        public static ServicesContainer Current
        {
            get
            {
                if (serviceContainer == null)
                {
                    lock (sync)
                    {
                        if (serviceContainer == null)
                            throw new InvalidOperationException("ServicesContainer must be initialized first");
                    }
                }
                return serviceContainer;
            }
        }

        public static void Initialize(WardeinBaseConfiguration wardeinBaseConfiguration)
        {
            if (serviceContainer == null)
            {
                lock (sync)
                {
                    if (serviceContainer == null)
                        serviceContainer = new ServicesContainer(wardeinBaseConfiguration);
                }
            }
        }

        #endregion

        #region Objects

        public static IAmWatcherPersistenceService WatcherPersistenceService() => Current.serviceProvider.GetService<IAmWatcherPersistenceService>();
        public static IAmWardeinHeartBeatPersistanceService WardeinHeartBeatPersistenceService() => Current.serviceProvider.GetService<IAmWardeinHeartBeatPersistanceService>();
        public static IAmMailConfigurationManager MailConfigurationManager() => Current.serviceProvider.GetService<IAmMailConfigurationManager>();
        public static IAmWardeinConfigurationManager WardeinConfigurationManager() => Current.serviceProvider.GetService<IAmWardeinConfigurationManager>();
        public static OracleConnectionConfiguration OracleConnectionConfiguration() => Current.serviceProvider.GetService<OracleConnectionConfiguration>();
        public static IAmUrlResponseManager UrlResponseManager() => Current.serviceProvider.GetService<IAmUrlResponseManager>();
        public static IOracleHelper OracleHelper() => Current.serviceProvider.GetService<IOracleHelper>();

        public static IAmNotificationService NotificationService(NotificationType notificationType)
        {
            var instanceResolver = Current.serviceProvider.GetService<Func<NotificationType, IAmNotificationService>>();
            return instanceResolver(notificationType);
        }

        public static IAmServiceManager ServiceManager(string serviceName, ServiceManagerType serviceManagerType)
        {
            var instanceResolver = Current.serviceProvider.GetService<Func<ServiceManagerType, string, IAmServiceManager>>();
            return instanceResolver(serviceManagerType, serviceName);
        }        

        #endregion
    }
}
