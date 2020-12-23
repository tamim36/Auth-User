﻿using Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net;

namespace Services
{
    public class MailService : IMailService
    {
        private readonly IOptions<MailSettings> mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings;
        }
        public async Task SendEmailAsync(string mail, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                string hostmail = "titnet4@gmail.com";
                email.Sender = MailboxAddress.Parse(hostmail);
                email.To.Add(MailboxAddress.Parse(mail));
                email.Subject = subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                string pass = "trust1315";
                smtp.Authenticate(hostmail, pass);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            } catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
