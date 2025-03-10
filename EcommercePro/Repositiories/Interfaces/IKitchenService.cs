using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IKitchenService :IGenaricService<KitchenServices>
    {
        public KitchenServices LastOrder();
        public IQueryable<KitchenServices> FilterRequestsBy(
        string projectName = null,string techniciName = null, string ClientName = null,
        string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts = null, int day = 0);
        public IQueryable<KitchenServices> GetRequestToCustomer(string CustomerId, string code, string Status, DateOnly date = default);
        public IQueryable<KitchenServices> GetRequestToTechninics(string TechincanId, string Status , int day=0);
        public Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData);
        public Task<(bool Succeeded, string[] Errors)> CloseByCode(CloseDto closeData);
        public Task<(bool Succeeded, string[] Errors)> CloseByNote(CloseByNoteDto closeData);
         public Task<(bool Succeeded, string[] Errors)> Reply(ReplyDto replyDto);
        public Task<(bool Succeeded, string[] Errors)> UploadAgreement(AgreemantFileDto AgreemantFileDto);
        public Task<(bool Succeeded, string[] Errors)> SendMessageToEmail(string Email, string Message);

        public Task<(bool Succeeded, string[] Errors)> Start(StartDto startDto);
        public IQueryable<KitchenServices> GetRequestToProjectsManager(List<int> projects, string projectName = null, string techniciName = null, string ClientName = null,
        string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts = null, int day = 0);

        public Task<(bool Succeeded, string[] Errors)> Delete(int id);


    }
}
