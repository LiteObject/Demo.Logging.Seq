namespace Demo.Logging.Seq.Services
{
    public interface IEmailService
    {
        public void SendEmail(string toAddress, string subjectLine, string messageBody);
    }
}
