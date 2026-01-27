namespace AI_Integration.Model
{
    public class MailSettings
    {
        public SMTP SMTP { get; set; }
        public string From { get; set; }
        public string Footer { get; set; }
        public string Sender { get; set; }
        
    }
    public class SMTP
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }

}