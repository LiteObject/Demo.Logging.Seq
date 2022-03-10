using Demo.Logging.Seq.Options;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net.Mail;

namespace Demo.Logging.Seq.Services
{
    /// <summary>
    /// More: https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        private readonly IOptions<MailOptions> _config;

        public EmailService(IOptions<MailOptions> config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;

            try
            {
                _ = _config.Value;

            }
            catch (OptionsValidationException ex)
            {
                foreach (var failure in ex.Failures)
                {
                    _logger.LogError(failure);
                }
            }
        }

        public void SendEmail(string toAddress, string subjectLine, string messageBody)
        {
            SmtpClient client = new SmtpClient(_config.Value.SmtpServer);

            // Specify the email sender. Create a mailing address that includes a UTF8 character in the display name.
            MailAddress from = new MailAddress(_config.Value.SenderAddress, "Do not " + (char)0xD8 + " reply", System.Text.Encoding.UTF8);
            // Set destinations for the email message.
            MailAddress to = new MailAddress(toAddress);
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = messageBody;
            // Include some non-ASCII characters in body and subject.
            string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subjectLine + " " + someArrows;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);

            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = $"{toAddress}-{DateTime.Now.Ticks}";
            client.SendAsync(message, userState);
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            var token = e.UserState?.ToString();

            if (e.Cancelled)
            {
                _logger.LogInformation("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                _logger.LogInformation("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                _logger.LogInformation("Message sent.");
            }
        }
    }
}
