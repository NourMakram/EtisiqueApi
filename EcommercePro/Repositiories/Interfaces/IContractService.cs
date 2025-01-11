
using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IContractService:IGenaricService<Contract> 
    {
        public Task<ContractResponse> GetByProjectId(int projectId);
        public string ContractSatuts (int satuts);
        public Task<(bool Succeeded, string[] Errors)> ChangeContractSatutsAsync(int id);
        public Task<(bool Succeeded, string[] Errors)> SendMessageAsync(int id);

    }
}
