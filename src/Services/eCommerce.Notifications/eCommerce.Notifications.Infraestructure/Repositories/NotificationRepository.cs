using eCommerce.Notifications.Core.Config;
using eCommerce.Notifications.Core.Contracts.Repositories;
using eCommerce.Notifications.Core.Objects.DbTypes;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace eCommerce.Notifications.Infraestructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(ILogger<NotificationRepository> logger) 
        {
            _logger = logger;
        }

        public Task SentNotificationFromGmail(NotificationEntity notificationentity)
        {
            try
            {
                var fromPassword = AppConfiguration.Configuration["AppConfiguration:Gmail:Password"].ToString();
                var fromAddress = new MailAddress(AppConfiguration.Configuration["AppConfiguration:Gmail:Email"].ToString(), "Ecommerce");
                var toAddress = new MailAddress(notificationentity.Email, notificationentity.Name);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = notificationentity.Subject,
                    Body = notificationentity.Body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar notificación: {notificationentity.Subject}, {notificationentity.Email}");
            }
            return Task.CompletedTask;
        }
    }
}
