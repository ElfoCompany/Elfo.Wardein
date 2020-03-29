using Elfo.Wardein.Abstractions;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core.Helpers;
using NLog;
using System;
using System.Net;
using System.Net.Mail;
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

            log.Debug("Sending email to {0}", recipientAddress);
            GetSmtpClient().Send(GetMailMessage());
            log.Debug("Email sent to {0}", recipientAddress);

            await Task.CompletedTask;

            #region Local functions

            MailConfiguration GetMailConfiguration()
            {
                var config = ServicesContainer.MailConfigurationManager()?.GetConfiguration();

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

                SmtpDeliveryMethod GetSmtpDelivertyMethod() => (SmtpDeliveryMethod)mailConfiguration.DeliveryMethod;

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
                    From = new MailAddress(mailConfiguration.FromAddress, GetFromDisplayName()),
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

            string GetFromDisplayName()
            {
                if (!string.IsNullOrWhiteSpace(mailConfiguration.FromDisplayName))
                    return mailConfiguration.FromDisplayName;
                else
                    return $"Elfo.Wardein from {HostHelper.GetName()}";
            }

            #endregion
        }
    }
}
