using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService
    {
        IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            string host = configuration["SmtpSettings:Host"]!;
            int port = Convert.ToInt32(configuration["SmtpSettings:Port"]);
            string email = configuration["SmtpSettings:Email"]!;
            string password = configuration["SmtpSettings:Password"]!;
            string displayName = configuration["SmtpSettings:DisplayName"]!;

            SmtpClient smtpClient = new SmtpClient(host);

            smtpClient.Port = port;
            smtpClient.Credentials = new NetworkCredential(email, password);
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(email, displayName);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
    }
}
