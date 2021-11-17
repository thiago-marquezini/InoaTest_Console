using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InoaTest_Console
{
    class SMTPSettings
    {
        public string Destinatario { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPass { get; set; }
        public bool SMTPSSL { get; set; }
    }

    class B3AtivoMail
    {
        private string Settings = "appsettings.json";
        private string Section  = "MailSettings";
        private SMTPSettings SMTP = new SMTPSettings();

        public B3AtivoMail(string Settings, string Section)
        {
            this.Settings = Settings;
            this.Section  = Section;
        }

        public bool Send(string Title, string Message)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile(this.Settings, optional: false, reloadOnChange: true).Build();
            Configuration.GetSection(this.Section).Bind(SMTP);

            var SMTPClient = new SmtpClient(SMTP.SMTPHost)
            {
                Port = SMTP.SMTPPort,
                Credentials = new NetworkCredential(SMTP.SMTPUser, SMTP.SMTPPass),
                EnableSsl = SMTP.SMTPSSL
            };

            SMTPClient.Send(SMTP.SMTPUser, SMTP.Destinatario, Title, Message);

            return true;
        }
    }
}
