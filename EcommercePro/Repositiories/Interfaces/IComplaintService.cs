using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IComplaintService:IGenaricService<Complaint>
    {
        public Complaint LastOrder();
        public IQueryable<ComplaintDto.ComplaintDtoForShow> FilterRequestsBy(string techniciId = null,int Code=0,
            string Status = null, string ClientName = null, string ProjectName = null, int day = 0, int week=0,int Year=0 , int Month=0);

        public ComplaintDto.ComplaintDtoForShow Get(int id);
        public Task<(bool Succeeded, string[] Errors)> Add(ComplaintDto.ComplaintDtoForAdd complaintDto);
        public Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData);
        public Task<(bool Succeeded, string[] Errors)> Close(CommonPartsmanagmentDto closeData);
        public Task<(bool Succeeded, string[] Errors)> PartialClose(CommonPartsmanagmentDto PartialCloseData);
 
        public IQueryable<ComplaintDto.ComplaintDtoForShow> GetTechincanRequest(string techincanId, string stauts, int day = 0);
        public Task<(bool Succeeded, string[] Errors)> Delete(int Id);

        public Task<(bool Succeeded, string[] Errors)> RequestManagment(RequestManagement requestManagement);

        public IQueryable<ComplaintDto.ComplaintDtoForShow> GetRequestToProjectsManager(List<int> projects, string techniciId = null, int Code = 0,
            string Status = null, string ClientName = null, string ProjectName = null, int day = 0, int week = 0, int Year = 0, int Month = 0);

    }
}
