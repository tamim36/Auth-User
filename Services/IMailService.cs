using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
