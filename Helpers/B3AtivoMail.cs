using System;
using System.Net;
using System.Net.Mail;

namespace InoaTest_Console.Helpers
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
        private SMTPSettings   SMTP;
        private SmtpClient     SMTPClient;

        public B3AtivoMail(SMTPSettings refSMTP)
        {
            SMTP = refSMTP;

            SMTPClient = new SmtpClient(SMTP.SMTPHost)
            {
                Port = SMTP.SMTPPort,
                Credentials = new NetworkCredential(SMTP.SMTPUser, SMTP.SMTPPass),
                EnableSsl = SMTP.SMTPSSL
            };
        }

        public void Send(string Title, string Message)
        {
            try
            {
                SMTPClient.Send(SMTP.SMTPUser, SMTP.Destinatario, Title, Message);

            } catch (Exception E)
            {
                throw new ArgumentException("Mail: " + E.Message);
            }
        }
    }
}
