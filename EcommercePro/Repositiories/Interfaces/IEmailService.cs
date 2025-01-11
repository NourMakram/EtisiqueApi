using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IEmailService
    {
        public Task<(bool Succeeded, string MessageError)> SendEmailAsync(string Email, string Meassage, string Subject);


    }
}
