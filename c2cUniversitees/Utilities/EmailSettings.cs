namespace c2cUniversitees.Utilities
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587; 
        public string SenderName { get; set; } = "سوق الجامعة";
        public string SenderEmail { get; set; } = "Your-Actual-Gmail@gmail.com"; 
        public string SenderPassword { get; set; } = "Your-App-Specific-Password"; 
    }
}
