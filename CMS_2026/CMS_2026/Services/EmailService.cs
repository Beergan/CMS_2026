using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CMS_2026.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:Username"] ?? "";
            _smtpPassword = _configuration["Email:Password"] ?? "";
            _fromEmail = _configuration["Email:FromEmail"] ?? _smtpUsername;
            _fromName = _configuration["Email:FromName"] ?? "CMS System";
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_fromEmail, _fromName);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtml;

                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = true;
                        await client.SendMailAsync(mail);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SendMailAsync(string subject, string body, string to)
        {
            var result = await SendEmailAsync(to, subject, body);
            return result ? "success" : "error";
        }
    }
}

