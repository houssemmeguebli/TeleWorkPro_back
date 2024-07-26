using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using TTProject.Core.Interfaces;

   public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.office365.com";
        private readonly int _smtpPort = 587; // Port for TLS
        private readonly string _username;
        private readonly string _password;

        public EmailService(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("TTAPP", _username));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html")
            {
                Text = $"<strong>{message}</strong>"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to send email", ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }