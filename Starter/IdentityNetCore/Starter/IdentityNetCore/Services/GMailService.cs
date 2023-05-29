using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IdentityNetCore.Models;

namespace IdentityNetCore.Services;

public class GMailService : IEMailService
{
    private readonly Configuration _configuration;

    public GMailService(Configuration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EMail eMail)
    {
        using (MailMessage mm = new MailMessage())
        {
            mm.From = new MailAddress(eMail.From); //--- Email address of the sender
            mm.To.Add(_configuration.EmailAccount); //---- Email address of the recipient.
            mm.Subject = eMail.Subject; //---- Subject of email.
            mm.Body = $"From: {eMail.From}\n\n{eMail.Body}"; //---- Content of email.
            mm.IsBodyHtml = false; //---- To specify wether email body contains HTML tags or not.

            using SmtpClient smtp = new SmtpClient(_configuration.EmailHost, _configuration.EmailPort);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true; //---- Specify whether host accepts SSL Connections or not.
            //---Your Email and password
            NetworkCredential NetworkCred =
                new NetworkCredential(_configuration.EmailAccount, _configuration.EmailPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            await smtp.SendMailAsync(mm);
        }
    }
}
