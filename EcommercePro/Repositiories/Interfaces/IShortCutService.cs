using EcommercePro.Models;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IShortCutService
    {
        (bool Succeeded, string[] Errors) Add(Shrt shortCut);
        public Shrt get(string code);

    }
}
