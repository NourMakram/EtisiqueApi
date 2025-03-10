using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IEmergencyService:IGenaricService<Emergency>
    {
        public EmergencyRequestDto.EmergencyRequestToShow GetRequest(int RequestId);
        public Emergency LastOrder();
        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> FilterRequestsBy(string projectName = null, int Type=0,
            string techniciName = null, string ClientName = null,
            string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day = 0
            , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default);
        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToCustomer(string CustomerId, string code, string Status, DateOnly date = default);
        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToTechninics(string TechincanId, string Status, int day);
        public Task<(bool Succeeded, string[] Errors)> CloseRequest(CloseDto close);
        public Task<(bool Succeeded, string[] Errors)> TransferRequest(TransferDto transfer);
        public Task<(bool Succeeded, string[] Errors)> PostponeRequest(PostponeDto Postpone);
        public Task<(bool Succeeded, string[] Errors)> ReplyRequest(ReplyDto replyDto);
        public Task<(bool Succeeded, string[] Errors)> StartRequest(StartDto startDto);
        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToProjectsManager(List<int> projects, string projectName = null, int type = 0,
            string techniciName = null, string ClientName = null,
            string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day = 0, int week = 0,
            int year = 0, int month = 0, DateOnly from = default, DateOnly to = default);
        public Task<(bool Succeeded, string[] Errors)> Add(EmergencyRequestDto.EmergencyRequestToAdd emergencyRequestToAdd);
        public Task<(bool Succeeded, string[] Errors)> Delete(int id);


    }

}
