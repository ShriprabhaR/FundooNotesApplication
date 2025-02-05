using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CommonLayer.Models
{
    public class SendForCollaborate
    {

        public string SendMail(string ToEmail, int NotesId, string collaborator)
        {
            string FromEmail = "raichurprabha1@gmail.com";
            MailMessage message = new MailMessage(FromEmail, ToEmail);
            string MailBody = "Hii! "+ToEmail+" You have collaboration with Notes :" +NotesId;
            message.Subject = "Collaborated by person "+collaborator;
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential credential = new NetworkCredential("raichurprabha1@gmail.com", "xqyv xmyw qisx baci");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true; //not using default credential
            smtpClient.Credentials = credential;  //

            smtpClient.Send(message);
            return ToEmail;

        }
    }
}
