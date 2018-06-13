using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.ConfigurationReader;
using Elfo.Wardein.Core.Model;
using Elfo.Wardein.Core.NotificationService;
using Elfo.Wardein.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core
{
    public class ServicesContainer
    {
        #region Local Variables

        private static object sync = new object();
        private static volatile ServicesContainer currentInstance;
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
            serviceProvider = new ServiceCollection()
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
                .AddTransient<WardeinInstance>()
                .BuildServiceProvider();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the istance as singleton
        /// </summary>
        public static new ServicesContainer Current
        {
            get
            {
                if (currentInstance == null)
                {
                    lock (sync)
                    {
                        if (currentInstance == null)
                            currentInstance = new ServicesContainer();
                    }
                }
                return currentInstance;
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

        public static WardeinInstance WardeinInstance => Current.serviceProvider.GetService<WardeinInstance>();

        #endregion
    }
}
