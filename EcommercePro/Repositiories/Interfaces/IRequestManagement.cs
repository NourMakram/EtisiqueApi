using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IRequestManagement :IGenaricService<RequestManagement>
    {
        public List<RequestManagement> GetRequestManagements(int requestId, int serviceType);
        public  Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData, int type);
        public Task<(bool Succeeded, string[] Errors)> Reply(ReplyDto replyDto, int type); 
        public Task<(bool Succeeded, string[] Errors)> Postpone(PostponeDto PostponeDto, int type);
        public Task<(bool Succeeded, string[] Errors)> CloseMehtod1(CloseDto close, int type);
        public Task<(bool Succeeded, string[] Errors)> CloseMehtod2(CloseByNoteDto close, int type);
        public Task<(bool Succeeded, string[] Errors)>Start(StartDto start, int type);

    }
}
