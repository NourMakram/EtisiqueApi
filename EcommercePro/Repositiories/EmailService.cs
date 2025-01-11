using EtisiqueApi.DTO;
using EtisiqueApi.Repositiories.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EtisiqueApi.Repositiories
{
    public class EmailService : IEmailService
    {
        private IConfiguration configuration ;
        private EmailSettings settings ;
        public EmailService(IConfiguration _configuration) {

            configuration = _configuration;
           settings = new EmailSettings();
            this.configuration.GetSection("emailSettings").Bind(settings);

        }
        public async Task<(bool Succeeded, string MessageError)> SendEmailAsync(string Email, string Meassage , string Subject)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(settings.host, settings.port);
                    client.Authenticate(settings.FromEmail, settings.password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = $"{Meassage}",
                        TextBody = "wellcome",
                    };
                    var message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    message.From.Add(new MailboxAddress("Etsaq", settings.FromEmail));
                    message.To.Add(new MailboxAddress(Subject, Email));
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
                //end of sending email
                return (true, "Success To Send Code");
            }
            catch (Exception ex)
            {
                return (false, "Faild To Send Code");
            }
        }
    }
}
