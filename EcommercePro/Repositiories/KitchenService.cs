using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stripe;
using static EcommercePro.Models.Constants;
using Stripe.FinancialConnections;
using System.Runtime.InteropServices;

namespace EtisiqueApi.Repositiories
{
    public class KitchenService : GenaricService<KitchenServices>, IKitchenService
    {
        private readonly Context _context;
        private IAcountService _acountService;
        private IRequestManagement _RequestManagmentService;
        private IEmailService _EmailService;
        private MessageSender2.IMessageSender _messageSender;
        private IShortCutService _shortCutService;

        public KitchenService(Context context, IAcountService acountService, IShortCutService shortCutService,
            IRequestManagement requestManagmentService, IEmailService emailService, MessageSender2.IMessageSender messageSender) : base(context)
        {

            _context = context;
            _acountService = acountService;
            _RequestManagmentService = requestManagmentService;
            _EmailService = emailService;
            _messageSender = messageSender;
            _shortCutService = shortCutService;
        }

        public IQueryable<KitchenServices> FilterRequestsBy(string projectName = null,
            string techniciId = null, string ClientName = null, string BuildingName = null,
            int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts = null , int day=0)
        {

            var Requests = _context.KitchenServices.AsNoTracking()
                       .Include(R => R.Project)
                      .Include(R => R.Technician)
                      .Include(R => R.Customer)
                      .ThenInclude(R => R.ApplicationUser)
                       .OrderByDescending(s => s.id)
                       .AsQueryable();
             if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
             }

             if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
             if (ClientName != null)
            {
                Requests = Requests.Where(R => R.ClientName.Contains(ClientName));

            }
            if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == (BuildingName));

            }
            if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));

            }
            if (UnitNo != 0)
            {
                Requests = Requests.Where(R => R.UnitNo == UnitNo);

            }
             if (ClientPhone != null)
            {
                Requests = Requests.Where(R => R.ClientPhone.Contains(ClientPhone));
            }
             if (Stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Stauts);
            }
             if(day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.CreatedDate) == currentData);

            }
            return Requests;

        }

        public IQueryable<KitchenServices> GetRequestToCustomer(string CustomerId, string code, string Status, DateOnly date = default)
        {
            var Request = _context.KitchenServices.AsNoTracking()
                      .Include(R => R.Project)
                      .Include(R => R.Technician)
                      .Include(R => R.Customer)
                      .ThenInclude(R => R.ApplicationUser)
                      .Where(K => K.Customer.UserId == CustomerId)
                      .OrderByDescending(R=>R.id)
                      .AsQueryable();
            if (code != null)
            {
                Request = Request.Where(R=>R.RequestCode.ToString().Contains(code));
            }
            else if (Status != null)
            {
                if (Status == "0")
                {
                    Request = Request.Where(R => R.RequestStuatus != "اغلاق")
                              .AsQueryable();
                }
                else
                {
                    Request = Request.Where(R => R.RequestStuatus == "اغلاق").AsQueryable();
                }
            }
            else if (date != default)
            {
                Request = Request
                      .Where(R => (date != default && (R.CreatedDate.Day == date.Day && R.CreatedDate.Month ==
                            date.Month && R.CreatedDate.Year == date.Year))).AsQueryable();
            }
            return Request;

        }

        public IQueryable<KitchenServices> GetRequestToTechninics(string TechincanId, string Status ,int day=0)
        {
            var Requests = _context.KitchenServices.AsNoTracking()
                .Where(K => K.TechnicianId == TechincanId)
                .OrderByDescending(R => R.id)
                      .AsQueryable(); ;
             if (Status != null)
            {
                Requests = Requests.Where(R=>R.RequestStuatus == Status);

            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.CreatedDate) == currentData);

            }
            return Requests;
        }

        public KitchenServices LastOrder()
        {
            return _context.KitchenServices.AsNoTracking().OrderBy(r => r.CreatedDate).LastOrDefault();
        }
        public override async Task<KitchenServices> GetByIdAsync(int id)
        {
            return await _context.KitchenServices.AsNoTracking()
                      .Include(R => R.Project)
                      .Include(R => R.Technician)
                      .Include(R => R.Customer)
                      .ThenInclude(R => R.ApplicationUser)
                      .Include(R => R.Location)
                      .FirstOrDefaultAsync(R => R.id == id);

        }

        public async Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData)
        {
            //get Request
            KitchenServices Request = await GetByIdAsync(tranferData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            var result = await _RequestManagmentService.Transfer(tranferData, (int)ServiceTypeEnum.KitchenService);

            if (!result.Succeeded)
            {

                return (false, result.Errors);

            }
            var currentDate = DateTime.UtcNow;
            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            Request.RequestStuatus = "جارى المعالجة";
            Request.TechnicianId = tranferData.techiancalId;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);
        }

        public async Task<(bool Succeeded, string[] Errors)> CloseByCode(CloseDto closeData)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                KitchenServices Request = await GetByIdAsync(closeData.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                if (Request.CloseCode != closeData.Code)
                {
                    return (false, new string[] { "Invaild Code" });

                }
                var result = await _RequestManagmentService.CloseMehtod1(closeData, (int)ServiceTypeEnum.KitchenService);

                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                Request.RequestStuatus = "اغلاق";
                TimeSpan resultDate = (dateAfter3Hours - Request.CreatedDate);

                TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
                Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

                var result2 = Update(Request);

                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);

                }

                string url = $"https://etsaq.com/app/clientPage/Evaluation/Service/{Request.id}/{Request.RequestCode}/{(int)ServiceTypeEnum.KitchenService}/{Request.TechnicianId}";

                 
                var short1 = new Shrt()
                {
                    Id = DateTime.UtcNow.Ticks.ToString(),

                    Url = url
                };
                var Shortresult = _shortCutService.Add(short1);
                if (!Shortresult.Succeeded)
                {
                    return (false, Shortresult.Errors);
                }

                var Message = MessageSender2.Messages.Close(Request.ClientName, short1.Id, Request.RequestCode.ToString());


                var SendMessageResult1 = await _messageSender.Send3Async(Request.ClientPhone, Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                var SendMessageResult = await SendMessageToEmail(Request.Customer.ApplicationUser.Email, Message);
                if (!SendMessageResult.Succeeded)
                {
                    _RequestManagmentService.Rollback(trans);

                    return (SendMessageResult.Succeeded, SendMessageResult.Errors);

                }
                _RequestManagmentService.Commit(trans);
                return (true, SendMessageResult.Errors);

            }
            catch(Exception ex)
            {
                _RequestManagmentService.Rollback(trans);

                return (false, new string[] { "Faild To Close the Request" });
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> Reply(ReplyDto replyDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                KitchenServices Request = await GetByIdAsync(replyDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }

                var result = await _RequestManagmentService.Reply(replyDto, (int)ServiceTypeEnum.KitchenService);

                if (!result.Succeeded)
                {

                    return (false, result.Errors);

                }
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                Request.RequestStuatus = "اغلاق";
                TimeSpan resultDate = (dateAfter3Hours - Request.CreatedDate);

                TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
                Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

                var result2 = Update(Request);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);

                }
                var SendMessageResult1 = await _messageSender.Send3Async(Request.ClientPhone, replyDto.Message, null);
                if (!SendMessageResult1)
                {
                    return (false,new string[] { "Faild To send Message" });


                }
                var SendMessageResult = await SendMessageToEmail(Request.Customer.ApplicationUser.Email, replyDto.Message);

                _RequestManagmentService.Commit(trans);

                return (SendMessageResult.Succeeded, SendMessageResult.Errors);
            }
            catch (Exception ex)
            {

                return (false, new string[] { ex.Message });

            }
        }

        public async Task<(bool Succeeded, string[] Errors)> UploadAgreement(AgreemantFileDto AgreemantFileDto)
        {

            KitchenServices Request = await GetByIdAsync(AgreemantFileDto.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            if (Request.AgreementFile == null)
            {
                //genetate Code
                Random generator = new Random();
                string CloseCode = generator.Next(0, 10000).ToString("D4");
                var Message = MessageSender2.Messages.NewRequestMsg(Request.ClientName, Request.RequestCode, CloseCode);

                var SendMessageResult1 = await _messageSender.Send3Async(Request.ClientPhone, Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                var results = await _EmailService.SendEmailAsync(Request.Customer.ApplicationUser.Email, Message, "طلب جديد");
                if (!results.Succeeded)
                {
                    return (false, new string[] { results.MessageError });

                }
                Request.CloseCode = CloseCode;

            }

            Request.AgreementFile = AgreemantFileDto.filePath;
            var result2 = Update(Request);
            if (!result2.Succeeded)
            {
                return (false, result2.Errors);

            }
            return (true, null);


        }
        public async Task<(bool Succeeded, string[] Errors)> SendMessageToEmail(string Email, string Message)
        {
            var result = await _EmailService.SendEmailAsync(Email, Message, "اتساق");
            return (result.Succeeded, new string[] { result.MessageError });
        }

        public async Task<(bool Succeeded, string[] Errors)> CloseByNote(CloseByNoteDto closeData)
        {
            KitchenServices Request = await GetByIdAsync(closeData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }

            var result = await _RequestManagmentService.CloseMehtod2(closeData, (int)ServiceTypeEnum.KitchenService);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            Request.RequestStuatus = "اغلاق";
             TimeSpan resultDate = (dateAfter3Hours - Request.CreatedDate);

            TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
            Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }


        public async Task<(bool Succeeded, string[] Errors)> Start(StartDto startDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                KitchenServices Request = await GetByIdAsync(startDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                string Message = $"تم بدء الطلب";
                var messageToClient = MessageSender2.Messages.StartRequest(Request.ClientName, Request.Technician.FullName, Request.Technician.PhoneNumber, Request.RequestCode);

                var result = await _RequestManagmentService.Start(startDto, (int)ServiceTypeEnum.KitchenService);

                if (!result.Succeeded)
                {

                    return (false, result.Errors);

                }
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                Request.RequestStuatus = "جارى التنفيذ";
                var result2 = Update(Request);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);

                }
                var SendMessageResult = await _messageSender.Send3Async(Request.ClientPhone, messageToClient + DateTime.UtcNow.Ticks.ToString(), null);
                if (!SendMessageResult)
                {
                    _RequestManagmentService.Rollback(trans);

                    return (false, new string[] { "Faild To send Message" });
                }
                _RequestManagmentService.Commit(trans);

                return (true, null);
            }
            catch (Exception ex)
            {
                _RequestManagmentService.Rollback(trans);

                return (false, new string[] { ex.Message });
            }
        }

        public IQueryable<KitchenServices> GetRequestToProjectsManager(List<int> projects, string projectName = null,
            string techniciId = null, string ClientName = null, string BuildingName = null,
            int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts = null, int day = 0)
        {

            //List<int> projects = _acountService.GetUserProjects(UserId);

            var Requests = _context.KitchenServices.AsNoTracking()
                       .Include(R => R.Project)
                      .Include(R => R.Technician)
                      .Include(R => R.Customer)
                      .ThenInclude(R => R.ApplicationUser)
                      .Where(R => projects.Any(userProject => userProject == R.projectId))
                            .OrderByDescending(R => R.id)
                            .AsQueryable(); ;

            if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }

             if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
             if (ClientName != null)
            {
                Requests = Requests.Where(R => R.ClientName.Contains(ClientName));

            }
             if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == (BuildingName));

            }
             if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));

            }
             if (UnitNo != 0)
            {
                Requests = Requests.Where(R => R.UnitNo == UnitNo);

            }
             if (ClientPhone != null)
            {
                Requests = Requests.Where(R => R.ClientPhone.Contains(ClientPhone));
            }
             if (Stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Stauts);
            }
             if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.CreatedDate) == currentData);

            }
            return Requests;
        }

        public async Task<(bool Succeeded, string[] Errors)> Delete(int id)
        {
            try
            {
                KitchenServices KitchenServices = await GetByIdAsync(id);
                if (KitchenServices != null)
                {
                    _context.KitchenServices.Remove(KitchenServices);
                    await _context.SaveChangesAsync();
                    return (true, null);
                }
                return (false, new string[] { "can`t delete this Reuest" });

            }
            catch (Exception ex)
            {
                return (false, new string[] { "can`t delete this Reuest" });
            }

        }
    }
}
