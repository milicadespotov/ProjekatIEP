using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace IepWebApp
{
    public static class Mailer
    {
        public static void sendMail(string toUser, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("mdespotovic333@gmail.com");
            mail.To.Add(toUser);
            mail.Subject = subject;
            mail.Body = body;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("mdespotovic333@gmail.com", "despotovicmilica");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}