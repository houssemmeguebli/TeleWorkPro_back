using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;
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
    public async Task SendEmailsAsyncList(List<string> emails, string subject, string body)
    {
        // Create a new SMTP client
        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            try
            {
                // Connect to the SMTP server
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_username, _password);

                // Loop through each email address and send an individual email
                foreach (var email in emails)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("TTAPP", _username));
                    message.To.Add(new MailboxAddress("", email));
                    message.Subject = subject;

                    // Set the email body as HTML
                    message.Body = new TextPart("html")
                    {
                        Text = body
                    };

                    // Send the email
                    await client.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions here
                Console.WriteLine($"Error sending emails: {ex.Message}");
                throw;
            }
            finally
            {
                // Disconnect from the SMTP server
                await client.DisconnectAsync(true);
            }
        }
    }


}