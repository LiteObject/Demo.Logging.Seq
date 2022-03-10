namespace Demo.Logging.Seq.Options
{
    /// <summary>
    /// More: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
    /// </summary>
    public class MailOptions
    {
        public const string Mail = "Email";

        public string SmtpServer { get; set; } = String.Empty;

        public int SmtpPort { get; set; } = 25;

        public string SenderAddress { get; set; } = String.Empty;
    }
}
