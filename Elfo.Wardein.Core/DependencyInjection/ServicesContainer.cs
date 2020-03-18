using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Abstractions.Services.Models;
using Elfo.Wardein.Core.ConfigurationReader;
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

        #endregion

        #region Constructor

        protected ServicesContainer()
        {
            Configure();
        }

        #endregion

        #region Configurations

        protected void Configure()
        {

            serviceCollection = serviceCollection = new ServiceCollection()
                .AddSingleton<Func<string, IAmMailConfigurationManager>>(sp => filePath => new MailConfigurationManagerFromJSON(filePath))
                .AddSingleton<Func<string, IAmWardeinConfigurationManager>>(sp => filePath => new WardeinConfigurationManagerFromJSON(filePath))
                .AddTransient<Func<string, IAmPersistenceService<WindowsServiceStats>>>(sp => filePath => new WindowsServiceStatsPersistenceInJSON(filePath))
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
                            serviceContainer = new ServicesContainer();
                    }
                }
                return serviceContainer;
            }
        }

        #endregion

        #region Objects

        public static IAmPersistenceService<WindowsServiceStats> PersistenceService(string filePath)
        {
            var instanceResolver = Current.serviceProvider.GetService<Func<string, IAmPersistenceService<WindowsServiceStats>>>();
            return instanceResolver(filePath);
        }

        public static IAmMailConfigurationManager MailConfigurationManager(string filePath)
        {
            var instanceResolver = Current.serviceProvider.GetService<Func<string, IAmMailConfigurationManager>>();
            return instanceResolver(filePath);
        }

        public static IAmWardeinConfigurationManager WardeinConfigurationManager(string filePath)
        {
            var instanceResolver = Current.serviceProvider.GetService<Func<string, IAmWardeinConfigurationManager>>();
            return instanceResolver(filePath);
        }

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
