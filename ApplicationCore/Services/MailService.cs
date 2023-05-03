using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Services
{
    public class MailService : IMailService
    {
        public void SendEmailAsync(ApplicationUser user, string password)
        {
            // personel şifresi mail'e gönderilecek. mail içeriği
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("hasanbaris.samur@bilgeadamboost.com, " +
                "ebubekir.tatlilioglu@bilgeadamboost.com, " +
                "mert.kacan@bilgeadamboost.com," +
                "cem.turkay@bilgeadamboost.com");
            mailMessage.From = new MailAddress("hr360official@gmail.com");
            mailMessage.Subject = "Şifrenizi belirleyin.";
            mailMessage.Body = $"Sayın {user.Name} {user.Surname}, <br> giriş bilgileriniz <br> Email : {user.Email} <br> <br> Password : {password} <br> Lütfen giriş yaparak şifrenizi değiştiriniz";
            mailMessage.IsBodyHtml = true;

            // mail gönderecek SmtpClianet sınıfını oluşturuyoruz
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("hr360official@gmail.com", "ddcpqgdbrzfehbam");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }
}
