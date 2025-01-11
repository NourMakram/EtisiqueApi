using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IGuaranteeService:IGenaricService<Guarantee>
    {
        public Task<List<GuaranteeResponse>> GetByProjectIdAsync(int projectId);
        public static string GuaranteeType(int guaranteeType, ISubServiceRequestService _subServiceRequestService)
        {
            string type = _subServiceRequestService.GetServices().FirstOrDefault(type => type.Id == guaranteeType)?.Name;
            return type;
        }

        public string GuaranteeStatus(int Status);
        public Task<(bool Succeeded, string[] Errors)> ChangeGuaranteeStatusAsync(int id);

    }
}
