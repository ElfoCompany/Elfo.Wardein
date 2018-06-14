using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.ConfigurationReader;
using Elfo.Wardein.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.NotificationService
{
    public class MailNotificationService : IAmNotificationService
    {
        #region Private variables
        private readonly string filePath = $@"{Const.BASE_PATH}Assets\MailConfig.json";
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        #endregion

        public async Task SendNotificationAsync(string recipientAddress, string notificationBody, string notificationTitle)
        {
            var mailConfiguration = ServicesContainer.MailConfigurationManager(filePath)?.GetConfiguration();

            if (mailConfiguration == null)
                throw new ArgumentNullException("Cannot find Mail SMTP Configuration");

            var fromAddress = new MailAddress(mailConfiguration.FromAddress, mailConfiguration.FromDisplayName);
            var toAddress = new MailAddress(recipientAddress, recipientAddress);

            var smtp = GetSmtpClient();
            SetSmtpCredentialsIfNeccesary();


            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = notificationTitle,
                Body = notificationBody
            };

            smtp.Send(mailMessage);

            #region Local functions

            SmtpClient GetSmtpClient() => new SmtpClient
            {
                Host = mailConfiguration.Host,
                Port = mailConfiguration.Port,
                EnableSsl = mailConfiguration.EnableSSL,
                DeliveryMethod = GetSmtpDelivertyMethod(),
                UseDefaultCredentials = mailConfiguration.UseDefaultCredentials
            };

            void SetSmtpCredentialsIfNeccesary()
            {
                if (!string.IsNullOrWhiteSpace(mailConfiguration.Password))
                    smtp.Credentials = new NetworkCredential(mailConfiguration.Username, mailConfiguration.Password);
            }

            SmtpDeliveryMethod GetSmtpDelivertyMethod() => Enum.Parse<SmtpDeliveryMethod>(mailConfiguration.DeliveryMethod);

            #endregion
        }
    }
}
