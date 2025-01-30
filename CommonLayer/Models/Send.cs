using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class Send
    {
        public string SendMail(string ToEmail, string Token)
        {
            string FromEmail = "raichurprabha1@gmail.com";
            MailMessage message = new MailMessage(FromEmail, ToEmail);
            string MailBody = "Token for the reset password:" + Token;
            message.Subject = "Token generated for resetting password";
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential credential = new NetworkCredential("raichurprabha1@gmail.com", "wdig abpw ztbh xmmt");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false; //not using default credential
            smtpClient.Credentials = credential;  //

            smtpClient.Send(message);
            return ToEmail;

        }
    }
}
