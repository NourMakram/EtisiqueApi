using EcommercePro.Models;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Org.BouncyCastle.Utilities;

namespace EtisiqueApi.Repositiories
{
    public class ShortCutService : IShortCutService
    {
        Context _context;
        public ShortCutService(Context context)
        {

            _context = context;

        }
        public (bool Succeeded, string[] Errors) Add(Shrt shortCut)
        {
            try
            {
                _context.Shrts.Add(shortCut);

                _context.SaveChanges();
                return (true, new string[] { });
            }
            catch (Exception ex)
            {

                return (false, new string[] { ex.Message });

            }

        }

        public Shrt get(string code)
        {
             return _context.Shrts.FirstOrDefault(s => s.Id == code);
         }
    }
}
