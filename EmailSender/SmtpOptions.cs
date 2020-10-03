using MailKit.Security;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public class SmtpOptions
    {
        public string? ServerName { get; set; }
        public int Port { get; set; }
        public SecureSocketOptions SocketOptions { get; set; }
        public string? Password { get; set; }
    }
}
