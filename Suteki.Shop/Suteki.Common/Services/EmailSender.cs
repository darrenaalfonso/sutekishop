﻿using System.Net.Mail;
using Suteki.Common.Services;
using Suteki.Common.Extensions;

namespace Suteki.Common.Services
{
    public class EmailSender : IEmailSender
    {
        readonly string smtpServer;
        readonly string fromAddress;

        public EmailSender(string smtpServer, string fromAddress)
        {
            this.smtpServer = smtpServer;
            this.fromAddress = fromAddress;
        }

        public void Send(string toAddress, string subject, string body)
        {
            Send(new[] { toAddress }, subject, body);
        }

        public void Send(string[] toAddress, string subject, string body)
        {
            // if the smtpServer is not configured, just return
            if (smtpServer == "") return;

            var message = new MailMessage
                                      {
                                          From = new MailAddress(fromAddress),
                                          Subject = subject,
                                          Body = body
                                      };
            toAddress.ForEach(a => message.To.Add(a));

            var smtpClient = new SmtpClient(smtpServer);
            smtpClient.Send(message);
        }
    }
}