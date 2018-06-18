using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.ConfigurationReader;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        #endregion

        public async Task SendNotificationAsync(string recipientAddress, string notificationBody, string notificationTitle)
        {
            var mailConfiguration = GetMailConfiguration();
            log.Debug(JsonConvert.SerializeObject(mailConfiguration));

            log.Info("Sending email to {0}", recipientAddress);
            GetSmtpClient().Send(GetMailMessage());
            log.Info("Email sent to {0}", recipientAddress);

            #region Local functions

            MailConfiguration GetMailConfiguration()
            {
                var config = ServicesContainer.MailConfigurationManager(Const.MAIL_CONFIG_PATH)?.GetConfiguration();

                if (config == null)
                    throw new ArgumentNullException("Cannot find Mail SMTP Configuration");

                return config;
            }

            SmtpClient GetSmtpClient()
            {
                var client = new SmtpClient(mailConfiguration.Host)
                {
                    Port = mailConfiguration.Port,
                    EnableSsl = mailConfiguration.EnableSSL,
                    DeliveryMethod = GetSmtpDelivertyMethod(),
                    UseDefaultCredentials = mailConfiguration.UseDefaultCredentials
                };
                SetSmtpCredentialsIfNeccesary();
                return client;

                #region Local functions

                SmtpDeliveryMethod GetSmtpDelivertyMethod() => Enum.Parse<SmtpDeliveryMethod>(mailConfiguration.DeliveryMethod);

                void SetSmtpCredentialsIfNeccesary()
                {
                    if (!string.IsNullOrWhiteSpace(mailConfiguration.Password))
                        client.Credentials = new NetworkCredential(mailConfiguration.Username, mailConfiguration.Password);
                }

                #endregion
            }

            MailMessage GetMailMessage()
            {
                var msg = new MailMessage()
                {
                    From = new MailAddress(mailConfiguration.FromAddress, mailConfiguration.FromDisplayName),
                    Subject = notificationTitle,
                    IsBodyHtml = true,
                    Body = notificationBody,
                };
                AddRecipients();
                return msg;

                #region Local Functions
                void AddRecipients()
                {
                    var recipients = recipientAddress.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var recipient in recipients)
                    {
                        msg.To.Add(recipient);
                    }
                }
                #endregion
            }

            #endregion
        }
    }
}
