using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IApartmentServicesRequestService:IGenaricService<ApartmentServicesRequest>
    {
        public List<ApartmentRequestDto> GetRequest(int RequestId);    
        public ApartmentServicesRequest LastOrder(bool hasGuarantee);
        public IQueryable<ApartmentRequestDto> FilterRequestsBy(bool hasGuarantee, string projectName = null, int TypeServiceId = 0, string techniciId = null,
         string ClientName = null, string BuildingName = null, int UnitNo = 0, string ClientPhone = null,
         string Status = null, string Code = null, int day = 0, int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int ConfRequest = 0);
         public IQueryable<ApartmentRequestDto> GetRequestToCustomer(int CustomerId, bool hasGuarantee, string code, string Status, DateOnly date = default);
        public IQueryable<ApartmentRequestDto> GetRequestToTechninics(string TechincanId, bool hasGuarantee, string Status,int day);
         public Task<(bool Succeeded, string[] Errors)> CloseRequest(CloseDto close);
        public Task<(bool Succeeded, string[] Errors)> TransferRequest(TransferDto transfer);
        public  Task<(bool Succeeded, string[] Errors)> PostponeRequest(PostponeDto Postpone);
        public Task<(bool Succeeded, string[] Errors)> ReplyRequest(ReplyDto replyDto);
        public Task<(bool Succeeded, string[] Errors)> SendMessageToEmail(string Email, string Message);
        public Task<(bool Succeeded, string[] Errors)> StartRequest(StartDto startDto);
        public IQueryable<ApartmentRequestDto> GetRequestToProjectsManager(string userId, List<int> projects, bool hasGuarantee, string projectName = null, int TypeServiceId = 0, string techniciId = null,
        string ClientName = null, string BuildingName = null, int UnitNo = 0, string ClientPhone = null,
        string Status = null, string Code = null, int day = 0, int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int ConfRequest = 0);


        public Task<(bool Succeeded, string[] Errors)> ApproveRequestMethod2(ApproveRequestDto approveRequest);
        public Task<(bool Succeeded, string[] Errors)> ApproveRequestMethod1(ApproveRequestDto approveRequest);

        public Task<(bool Succeeded, string[] Errors)> AcceptRequest(ReplyDto acceptRequest);
        public Task<(bool Succeeded, string[] Errors)> RefuseRequest(ReplyDto acceptRequest);

        public bool change();

    }
}
