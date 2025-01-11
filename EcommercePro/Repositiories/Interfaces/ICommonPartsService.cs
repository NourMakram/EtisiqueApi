using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface ICommonPartsService : IGenaricService<RequestsCommonParts>
    {
        public RequestsCommonParts LastOrder();
        public IQueryable<RequestsCommonParts> FilterRequestsBy(string projectName = null, int TypeServiceId = 0, string techniciId = null,
            string ManagerId = null, string BuildingName = null, string Status = null, string Code = null, bool IsUrget = false, int day = 0
            ,int week=0,int year=0,int month=0,DateOnly from=default , DateOnly to=default, int Note = 0);

        public IQueryable<RequestsCommonParts> GetRequestToProjectsManager(List<int> Projects, string projectName, int TypeServiceId, string techniciId,
         string ManagerId, string BuildingName, string Status, string Code, bool IsUrget = false, int day = 0
         , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int Note = 0);
        public CommonPartsResponse Get(int id);

        public Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData);
        public Task<(bool Succeeded, string[] Errors)> Close(CommonPartsmanagmentDto closeData);
        public  Task<(bool Succeeded, string[] Errors)> PartialClose(CommonPartsmanagmentDto PartialCloseData1,
        string ProcedureType, string RequestStuatus);
         public Task<(bool Succeeded, string[] Errors)> Comment(CommonPartsmanagmentDto CommentData);
        public Task<(bool Succeeded, string[] Errors)> Confirm (StartDto ConfirmDataDto);
        public Task<(bool Succeeded, string[] Errors)> Refuse(CloseByNoteDto RefuseDataDto);
        public Task<(bool Succeeded, string[] Errors)> Note(CloseByNoteDto NoteDataDto);
        public Task<(bool Succeeded, string[] Errors)> Up(CommonPartsmanagmentDto UpDataDto);
        public IQueryable<RequestsCommonParts> GetTechincanRequest(string techincanId, string stauts, int day=0);
        public IQueryable<RequestsCommonParts> GetManagerRequest(string managerId, string stauts, int day = 0);
        

    }

}
