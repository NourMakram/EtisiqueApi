using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Repositiories
{
    public class RequestManagmentService :GenaricService<RequestManagement> , IRequestManagement
    {
        Context _context;
        private IAcountService _acountService;
        private IRequestImage _requestImage;
        public RequestManagmentService(Context context,IRequestImage requestImage, IAcountService acountService) : base(context)
        {
            _context = context;
            _acountService = acountService;
            _requestImage = requestImage;

        }

        public List<RequestManagement> GetRequestManagements(int requestId , int serviceType )
        {
            return _context.RequestManagements.Include(M => M.CreatedBy).Include(M=>M.Attachments)
                .Where(M => M.RequestId == requestId && M.ServiceType == serviceType)
                .ToList();
        }

        public async Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData, int type )
        {
            try
            {
                //get Technician
                ApplicationUser userdb = await _acountService.GetUserByIdAsync(tranferData.techiancalId);
                if (userdb == null)
                {
                    return (false, new string[] { "Technician Not Found" });

                }
                string Message = $" تم تحويل الطلب الى  {userdb.FullName}";
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                RequestManagement Transfer = new RequestManagement()
                {
                    RequestId = tranferData.RequestId,
                    ProcedureType = "تحويل",
                    userId = tranferData.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = Message,
                    ServiceType = type,

                };
                var result = await AddAsync(Transfer);

                return (result.Succeeded, result.Errors);
            }
            catch (Exception ex)
            {

              return (false,new string[] { ex.Message });  
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> Reply(ReplyDto replyDto, int type)
        {
            try
            {
                string Message = $"'{replyDto.Message}'";

                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                RequestManagement Reply = new RequestManagement()
                {
                    RequestId = replyDto.RequestId,
                    ProcedureType = "رد",
                    userId = replyDto.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = Message,
                    ServiceType = type,

                };
                var result = await AddAsync(Reply);

                return (result.Succeeded, result.Errors);

            }
            catch (Exception ex)
            {

                return (false, new string[] { ex.Message });
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> Postpone(PostponeDto PostponeDto, int type)
        {
            try
            {
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                RequestManagement Postpone = new RequestManagement()
                {
                    RequestId = PostponeDto.RequestId,
                    ProcedureType = "تأجيل",
                    userId = PostponeDto.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = PostponeDto.Note,
                    ServiceType = type,
                    //Attachments = PostponeDto.filePath

                };
                var result = await AddAsync(Postpone);
                if (!result.Succeeded)
                {
                    return (result.Succeeded, result.Errors);

                }
                if (PostponeDto.formFiles != null && PostponeDto.formFiles.Count() > 0)
                {
                    var result1 = await _requestImage.Add(PostponeDto.formFiles, Postpone.Id, "Attchments");
                    if (!result1.Succeeded)
                    {
                        return (result1.Succeeded, result1.Errors);
                    }
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> CloseMehtod1(CloseDto close, int type)
        {
            try
            {
                string Message = $" تم اغلاق الطلب ";
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                RequestManagement Close = new RequestManagement()
                {
                    RequestId = close.RequestId,
                    ProcedureType = "اغلاق",
                    userId = close.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = Message,
                    ServiceType = type,
                   // Attachments= close.filePath


                };
                var result = await AddAsync(Close);
                if (!result.Succeeded)
                {
                    return (result.Succeeded, result.Errors);

                }

                if (close.formFiles != null && close.formFiles.Count() > 0)
                {
                  var result1 = await _requestImage.Add(close.formFiles, Close.Id, "Attchments");
                    if (!result1.Succeeded)
                    {
                        return (result1.Succeeded, result1.Errors);
                    }
                }

                return (true, null);

            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }
        }

        public  async Task<(bool Succeeded, string[] Errors)> CloseMehtod2(CloseByNoteDto close, int type)
        {
            try
            {
                string Message = close.Note;
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                RequestManagement Close = new RequestManagement()
                {
                    RequestId = close.RequestId,
                    ProcedureType = "اغلاق",
                    userId = close.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = Message,
                    ServiceType = type,

                };
                var result = await AddAsync(Close);

                return (result.Succeeded, result.Errors);

            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }
        }

        public  async Task<(bool Succeeded, string[] Errors)> Start(StartDto start, int type)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                RequestManagement SartData = new RequestManagement()
                {
                RequestId = start.RequestId,
                ProcedureType = "بدء",
                userId = start.userId,
                CreatedDate = dateAfter3Hours,
                Notes = "تم بدء الطلب",
                ServiceType = type,

               };
            var result = await AddAsync(SartData);

            return (result.Succeeded, result.Errors);

        }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message
    });
            }
        }
    }
}
