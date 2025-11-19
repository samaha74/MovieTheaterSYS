using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace MovieTheaterSYS.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bahaa.test99@gmail.com", "nfxl rbcy ithe gvrk")

            };

            return  client.SendMailAsync(

                new MailMessage(from: "bahaa.test99@gmail.com",
                                to: email,
                                subject: subject,
                                htmlMessage)
                {
                    IsBodyHtml = true
                }
            );
        }

    }
}
