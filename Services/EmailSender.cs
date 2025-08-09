using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FoodMarket.Services.Interfaces;

namespace FoodMarket.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtp = _config.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
            {
                Credentials = new NetworkCredential(smtp["UserName"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtp["UserName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            try
            {
                Console.WriteLine($"📤 Отправка письма на: {email}");
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("✅ Письмо успешно отправлено!");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("❌ Ошибка SMTP: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                    Console.WriteLine("➡ Доп. информация: " + smtpEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Общая ошибка при отправке письма: " + ex.Message);
            }
        }
    }
}