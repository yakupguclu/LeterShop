using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.MailServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class EmailSender : IEmailSender
    {
        private const string sendGridKey = "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";
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(sendGridKey, subject, htmlMessage, email);
        }
        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@letershop.com", "Leter Shop"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message

            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }


    }

}
