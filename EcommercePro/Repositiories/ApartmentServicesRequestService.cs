using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using static EcommercePro.Models.Constants;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace EtisiqueApi.Repositiories
{
    public class ApartmentServicesRequestService : GenaricService<ApartmentServicesRequest>,
        IApartmentServicesRequestService
    {
       
        private readonly Context _context;
        private IAcountService _acountService;
        private IRequestManagement _RequestManagmentService;
        private ISubServiceRequestService _subServiceRequestService;
        private IEmailService _emailService;
        private MessageSender2.IMessageSender _messageSender;
        private IShortCutService _shortCutService;
        private IRequestImage _requestImage;
        private IFileService _fileService;

        public ApartmentServicesRequestService(Context context, IAcountService acountService,IEmailService emailService,
            IRequestManagement RequestManagmentService, ISubServiceRequestService subServiceRequestService,
            MessageSender2.IMessageSender messageSender,IFileService fileService, IRequestImage requestImage, IShortCutService shortCutService) : base(context)
        {
           
            _context=context;
            _acountService=acountService;
            _RequestManagmentService=RequestManagmentService;
            _subServiceRequestService = subServiceRequestService;
            _emailService = emailService;
            _messageSender=messageSender;
            _shortCutService = shortCutService;
            _requestImage = requestImage;
            _fileService = fileService;



        }
        public override Task<ApartmentServicesRequest> GetByIdAsync(int id)
        {
            return _context.ApartmentServicesRequests.Include(R=>R.ApplicationUser).Include(r=>r.Customer).
                ThenInclude(r=>r.ApplicationUser).FirstOrDefaultAsync(R => R.id == id);
        }
        public IQueryable<ApartmentRequestDto> FilterRequestsBy(bool hasGuarantee,string projectName=null, int TypeServiceId=0,string techniciId=null,
            string ClientName=null,string BuildingName=null,int UnitNo=0 ,string ClientPhone=null ,
            string Status=null , string Code=null, int day=0, int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default,int ConfRequest = 0)
        {
            var requests = _context.ApartmentServicesRequests.AsNoTracking()
                    .Include(R => R.Project)
                    .Include(R => R.Customer)
                    .Include(R => R.Location)
                    .Include(R => R.ApplicationUser)
                    .Include(R => R.ServiceType)
                    .OrderByDescending(R => R.id)
                    .Where(R=>R.hasGuarantee==hasGuarantee)
                       .AsQueryable();
            if (projectName != null)
            {
                requests = requests.Where(R => R.Project.ProjectName.Contains(projectName));
             }
              if (TypeServiceId != 0)
            {
                requests = requests.Where(R => R.ServiceTypeId==TypeServiceId);

            }
              if (techniciId != null)
            {
                requests = requests.Where(R => R.TechnicianId == techniciId);

            }
              if (ClientName != null)
            {
                requests = requests.Where(R=>R.ClientName.Contains(ClientName));

            }
              if (BuildingName != null)
            {
                requests = requests.Where(R => R.BuildingName.Contains(BuildingName));

            }
              if (Code != null)
            {
                requests = requests.Where(R => R.RequestCode.ToString().Contains(Code));

            }
              if (Status != null)
            {
                requests = requests.Where(R => R.RequestStuatus == Status);

            }
              if(UnitNo != 0)
            {
                requests = requests.Where(R => R.UnitNo == UnitNo);

            }
              if (ClientPhone != null)
            {
                requests = requests.Where(R => R.ClientPhone.Contains(ClientPhone));

            }
              if(day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                requests = requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);


            }
              if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= startOfWeek && DateOnly.FromDateTime(R.DateOfVisit) < endOfWeek);

            }
              if (month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                requests = requests.Where(R => R.DateOfVisit.Month == CurrentMonth);

            }
              if (year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                requests = requests.Where(R => R.DateOfVisit.Year == CurrentYear);

            }
            if (from != default)
            {
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= from);

            }
            if (to != default)
            {
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) <= to && DateOnly.FromDateTime(R.DateOfVisit) >= from);


            }
            if (ConfRequest != 0)
            {
                if (ConfRequest == 1)
                {
                    requests = requests.Where(R => R.RequestStuatus == "جارى التعميد");

                }
                if (ConfRequest == 2)
                {
                    requests = requests.Where(R => R.RequestStuatus == "مقبول" && R.IsConfirmation == true );

                }
                if (ConfRequest == 3)
                {
                    requests = requests.Where(R => R.RequestStuatus == "مرفوض" && R.IsConfirmation == true );

                }
            }
            return requests
                    .Select(R => new ApartmentRequestDto()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        Period = R.Period,
                        ClientName = R.ClientName,
                        ClientPhone = R.ClientPhone,
                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy"),
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Address,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        ServiceType = R.ServiceType.Name,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        timeElasped = R.TimeElapsed,
                        Latitude = R.Location.Latitude,
                        Longitude = R.Location.Longitude,
                        CloseCode = R.CloseCode,
                        ProjectId=R.projectId
                        
                    }).AsQueryable(); ;

        }
        public List<ApartmentRequestDto> GetRequest(int RequestId)
        {
            List<string> images = _requestImage.GetImages(RequestId, (int)ServiceTypeEnum.ApartmentService);

            var SubServices = _subServiceRequestService.GetSubServices(RequestId,(int)ServiceTypeEnum.ApartmentService);

            var RequestVerivication = _context.ApartmentServicesVerifications.Include(R=>R.Approver)
                .Where(R => R.RequestId == RequestId)
                .Select(R=>new ServicesVerificationsDto()
                {
                  ApproverName=R.Approver.FullName,
                  IsApproved=R.IsApproved,
                  ApproverId=R.ApproverID,
                  Fees=R.Fees,
                  File=R.File,
                  Note=R.Note,
                  TimeElapsed=R.TimeElapsed
                }).FirstOrDefault();

		   List<ApartmentRequestDto> apartmentServicesRequest = _context.ApartmentServicesRequests.AsNoTracking()
                                    .Where(R => R.id == RequestId)
                                    .Include(R=>R.Project)
                                    .Include(R=>R.Customer)
                                    .Include(R=>R.Location)
                                    .Include(R=>R.ApplicationUser)
                                    .Include(R=>R.ServiceType)
                                    .Select(R=>new ApartmentRequestDto()
                                    {
                                        id = R.id,
                                        RequestCode=R.RequestCode,
                                        projectName =R.Project.ProjectName,
                                        Period = R.Period,
                                        ClientName=R.ClientName,
                                        ClientPhone =R.ClientPhone,
                                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy"),
                                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        Address = R.Address,
                                         SubServices = SubServices,
                                        TechnicianName = R.ApplicationUser.FullName,
                                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                                        ServiceType = R.ServiceType.Name,
                                        RequestStuatus=R.RequestStuatus,
                                        Description=R.Description,
                                        UnitNo=R.Customer.UnitNo.ToString(),
                                        BulidingName=R.Customer.BulidingName,
                                         timeElasped = R.TimeElapsed,
                                        Latitude =R.Location.Latitude,
                                        Longitude=R.Location.Longitude,
                                        CloseCode=R.CloseCode,
                                        images=images,
                                        hasGuarante=R.hasGuarantee,
                                        servicesVerification = RequestVerivication
                                    }).ToList();
            return apartmentServicesRequest;

        }
        public IQueryable<ApartmentRequestDto> GetRequestToTechninics(string TechincanId, bool hasGuarantee, string Status, int day=0 )
        {
            
            var requests =  _context.ApartmentServicesRequests.AsNoTracking()
                            .Where(R => R.TechnicianId == TechincanId && R.hasGuarantee==hasGuarantee)
                            .AsQueryable();
            if (Status!=null)
            {
                requests = requests
                          .Where(R =>  R.RequestStuatus == Status)
                            .AsQueryable();
            }
             else if(day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                requests = requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData)
                            .AsQueryable();
            }
            return requests
                    .Include(R => R.Project)
                    .Include(R => R.Customer)
                    .Include(R => R.Location)
                    .Include(R => R.ApplicationUser)
                    .Include(R => R.ServiceType)
                    .OrderByDescending(R => R.id)
                    .Select(R => new ApartmentRequestDto()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        Period = R.Period,
                        ClientName = R.ClientName,
                        ClientPhone = R.ClientPhone,
                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy"),
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Address,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        ServiceType = R.ServiceType.Name,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        timeElasped = R.TimeElapsed,
                        Latitude = R.Location.Latitude,
                        Longitude = R.Location.Longitude,
                        CloseCode = R.CloseCode
                    }).AsQueryable();
        }
        public IQueryable<ApartmentRequestDto> GetRequestToCustomer(int CustomerId,bool hasGuarantee , string code, string Status, DateOnly date=default)
      {
            var requests = _context.ApartmentServicesRequests.AsNoTracking()
                            .Include(R => R.Project)
                            .Include(R => R.Customer)
                            .Include(R => R.Location)
                            .Include(R => R.ApplicationUser)
                            .Include(R => R.ServiceType)
                            .OrderByDescending(R => R.id)
                            .Where(R => R.CustomerId == CustomerId && R.hasGuarantee == hasGuarantee);
             if (code!=null && date != default)
            {
                requests = requests
                              .Where(R => R.RequestCode.ToString().Contains(code)
                              &&(DateOnly.FromDateTime(R.CreatedDate) == date))
                              .AsQueryable();

            }
            else  if (code != null)
            {
                requests = requests
                            .Where(R =>R.RequestCode.ToString().Contains(code))
                            .AsQueryable();
                                

            }
            else if(date !=default)
            {
                requests = requests
                            .Where(R => DateOnly.FromDateTime(R.CreatedDate) == date)
                            .AsQueryable();

            }
            else if (Status != null)
            {
                if (Status == "0")
                {
                    requests = requests
                            .Where(R => R.RequestStuatus != "اغلاق")
                            .AsQueryable();
                }
                else
                {
                    requests = requests.Where(R =>  R.RequestStuatus == "اغلاق")
                                                .AsQueryable();

                }
            }
             
            return requests
                    .Select(R => new ApartmentRequestDto()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        Period = R.Period,
                        ClientName = R.ClientName,
                        ClientPhone = R.ClientPhone,
                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy"),
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Address,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        ServiceType = R.ServiceType.Name,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        timeElasped = R.TimeElapsed,
                         Latitude = R.Location.Latitude,
                        Longitude = R.Location.Longitude,
                        CloseCode = R.CloseCode,
                        ProjectId = R.projectId

                    }).AsQueryable();
        }
        public async Task<(bool Succeeded, string[] Errors)> CloseRequest(CloseDto close)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                ApartmentServicesRequest Request = await GetByIdAsync(close.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            if (Request.CloseCode != close.Code)
            {
                return (false, new string[] { "Invaild Code" });

            } 
           
            var result = await _RequestManagmentService.CloseMehtod1(close,(int)ServiceTypeEnum.ApartmentService);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            Request.RequestStuatus = "اغلاق";
            TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
             TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
            Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);
            var result2 = Update(Request);
            
             if (!result2.Succeeded)
            {
                return (false, result2.Errors);

            }

                string url = $"/clientPage/Evaluation/Service/{Request.id}/{Request.RequestCode}/{(int)ServiceTypeEnum.ApartmentService}/{Request.TechnicianId}";

                var short1 = new Shrt()
                {
                    Id=DateTime.UtcNow.Ticks.ToString(),
                     Url =url 
                };
                var Shortresult = _shortCutService.Add(short1);
                if (!Shortresult.Succeeded)
                {
                    return (false, Shortresult.Errors);
                }

                var Message = MessageSender2.Messages.Close(Request.ClientName, short1.Id,Request.RequestCode.ToString());

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
                //return (true, null);


            }
            catch (Exception ex)
            {
             _RequestManagmentService.Rollback(trans);

                return (false, new string[] { "Faild To Close the Request" });
            }


        }
        public async Task<(bool Succeeded, string[] Errors)> PostponeRequest(PostponeDto Postpone)
        {
             ApartmentServicesRequest Request = await GetByIdAsync(Postpone.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }

            var result = await _RequestManagmentService.Postpone(Postpone, (int)ServiceTypeEnum.ApartmentService);
            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            
            Request.RequestStuatus = "تأجيل";
            Request.DateOfVisit =DateTime.Parse(Postpone.PostponeDate);
            var result2 = Update(Request);

            
            return (result2.Succeeded, result2.Errors);
 

        }
        public async Task<(bool Succeeded, string[] Errors)> TransferRequest(TransferDto transfer)
        {
            //get Request
            ApartmentServicesRequest Request = await GetByIdAsync(transfer.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            var result = await _RequestManagmentService.Transfer(transfer, (int)ServiceTypeEnum.ApartmentService);

            if (!result.Succeeded)
            {

                return (false, result.Errors);

            }
            var currentDate = DateTime.UtcNow;
            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            Request.RequestStuatus = "جارى المعالجة";
            Request.TechnicianId = transfer.techiancalId;
            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);


        }
        public ApartmentServicesRequest LastOrder(bool hasGuarantee)
        {
            return _context.ApartmentServicesRequests.AsNoTracking().OrderBy(r=>r.CreatedDate).LastOrDefault(R=>R.hasGuarantee==hasGuarantee);
        }
        public async Task<(bool Succeeded, string[] Errors)> ReplyRequest(ReplyDto replyDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                ApartmentServicesRequest Request = await GetByIdAsync(replyDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
               
                var result =await _RequestManagmentService.Reply(replyDto,(int)ServiceTypeEnum.ApartmentService);

                if (!result.Succeeded)
                {

                    return (false, result.Errors);

                }
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                Request.RequestStuatus = "اغلاق";
                var result2 = Update(Request);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);

                }

                var SendMessageResult1 = await _messageSender.Send3Async(Request.ClientPhone, replyDto.Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                var SendMessageResult2 = await SendMessageToEmail(Request.Customer.ApplicationUser.Email, replyDto.Message);

                _RequestManagmentService.Commit(trans);

                return (SendMessageResult2.Succeeded, SendMessageResult2.Errors);

            }
            catch(Exception ex)
            {
                _RequestManagmentService.Rollback(trans);

                return (false,new string[] { ex.Message });
            }
        }
        public async Task<(bool Succeeded, string[] Errors)>SendMessageToEmail(string Email ,string Message)
        {
           var result =await _emailService.SendEmailAsync(Email, Message, "اتساق");
            return (result.Succeeded,new string[] { result.MessageError });
        }
         public async Task<(bool Succeeded, string[] Errors)> StartRequest(StartDto startDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                ApartmentServicesRequest Request = await GetByIdAsync(startDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                string Message = $"تم بدء الطلب";
               

                var result = await _RequestManagmentService.Start(startDto, (int)ServiceTypeEnum.ApartmentService);

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
                var messageToClient = MessageSender2.Messages.StartRequest(Request.ClientName, Request.ApplicationUser.FullName, Request.ApplicationUser.PhoneNumber, Request.RequestCode);


                var SendMessageResult = await _messageSender.Send3Async(Request.ClientPhone, messageToClient, null);
                if (!SendMessageResult)
                {
                    _RequestManagmentService.Rollback(trans);

                    return (false, new string[] { "Faild To send Message" });
                }

                _RequestManagmentService.Commit(trans);

                return (true,null);

            }
            catch (Exception ex)
            {
                _RequestManagmentService.Rollback(trans);

                return (false, new string[] { ex.Message });
            }
        }
         public IQueryable<ApartmentRequestDto> GetRequestToProjectsManager(string userId,List<int> projects, bool hasGuarantee, string projectName = null, int TypeServiceId = 0, string techniciId = null,
            string ClientName = null, string BuildingName = null, int UnitNo = 0, string ClientPhone = null,
            string Status = null, string Code = null, int day = 0, int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default,int ConfRequest=0)
        {
              
            var requests = _context.ApartmentServicesRequests.AsNoTracking()
                            .Where(R =>projects.Any(userProject => userProject == R.projectId) && R.hasGuarantee == hasGuarantee)
                            .AsQueryable();

            if (projectName != null)
            {
                requests = requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
              if (TypeServiceId != 0)
            {
                requests = requests.Where(R => R.ServiceTypeId == TypeServiceId);

            }
              if (techniciId != null)
            {
                requests = requests.Where(R => R.TechnicianId == techniciId);

            }
              if (ClientName != null)
            {
                requests = requests.Where(R => R.ClientName.Contains(ClientName));

            }
              if (BuildingName != null)
            {
                requests = requests.Where(R => R.BuildingName.Contains(BuildingName));

            }
              if (Code != null)
            {
                requests = requests.Where(R => R.RequestCode.ToString().Contains(Code));

            }
              if (Status != null)
            {
                requests = requests.Where(R => R.RequestStuatus == Status);

            }
              if (UnitNo != 0)
            {
                requests = requests.Where(R => R.UnitNo == UnitNo);

            }
              if (ClientPhone != null)
            {
                requests = requests.Where(R => R.ClientPhone.Contains(ClientPhone));

            }
              if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                requests = requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);


            }
              if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= startOfWeek && DateOnly.FromDateTime(R.DateOfVisit) < endOfWeek);

            }
              if (month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                requests = requests.Where(R => R.DateOfVisit.Month == CurrentMonth);

            }
              if (year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                requests = requests.Where(R => R.DateOfVisit.Year == CurrentYear);

            }
            if (from != default)
            {
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= from);

            }
            if (to != default)
            {
                requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) <= to && DateOnly.FromDateTime(R.DateOfVisit) >= from);


            }
            if (ConfRequest != 0)
            {
                if (ConfRequest == 1)
                {
                    requests = requests.Where(R => R.RequestStuatus == "جارى التعميد" && R.ApartmentServicesVerifications.ApproverID==userId);

                }
                if (ConfRequest == 2)
                {
                    requests = requests.Where(R => R.RequestStuatus == "مقبول" && R.IsConfirmation==true && R.ApartmentServicesVerifications.ApproverID == userId);

                }
                if (ConfRequest == 3)
                {
                    requests = requests.Where(R => R.RequestStuatus == "مرفوض" && R.IsConfirmation == true && R.ApartmentServicesVerifications.ApproverID == userId);

                }
            }
            return requests
                    .Include(R => R.Project)
                    .Include(R => R.Customer)
                    .Include(R => R.Location)
                    .Include(R => R.ApplicationUser)
                    .Include(R => R.ServiceType)
                    .Include(R=>R.ApartmentServicesVerifications)
                    .OrderByDescending(R => R.id)
                    .Select(R => new ApartmentRequestDto()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        Period = R.Period,
                        ClientName = R.ClientName,
                        ClientPhone = R.ClientPhone,
                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy"),
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Address,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        ServiceType = R.ServiceType.Name,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        timeElasped = R.TimeElapsed,
                        Latitude = R.Location.Latitude,
                        Longitude = R.Location.Longitude,
                        CloseCode = R.CloseCode,
                        ProjectId=R.projectId
                    }).AsQueryable();
        }
        public bool change()
        {
            try
            {
                List<ApartmentServicesRequest> apartmentServicesRequests = _context.ApartmentServicesRequests.Where(R => R.hasGuarantee == null).ToList();
                foreach (var apartmentServicesRequest in apartmentServicesRequests)
                {
                    apartmentServicesRequest.hasGuarantee = true;
                }
                _context.UpdateRange(apartmentServicesRequests);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public async Task<(bool Succeeded, string[] Errors)> ApproveRequestMethod1(ApproveRequestDto approveRequest)
        {
            try
            {
                var request = _context.ApartmentServicesVerifications.FirstOrDefault(r => r.RequestId == approveRequest.RequestId && r.IsApproved==false);
                var imageUrl = "";

                if (approveRequest.FormFile != null)
                {
                    var result1 = await _fileService.SaveImage("Attachments", approveRequest.FormFile);
                    if (!result1.Succeeded)
                    {
                        return (result1.Succeeded, result1.Errors);
                    }
                    imageUrl = result1.imagePath;
                }

                if (request == null)
                {
                    var apartmentServices = new ApartmentServicesVerifications
                    {
                        ApproverID = approveRequest.ApproverId,
                        RequestId = approveRequest.RequestId,
                        Fees = approveRequest.Fees,
                        Note = approveRequest.Note,
                        IsApproved = false,
                        File = imageUrl
                    };

                    _context.ApartmentServicesVerifications.Add(apartmentServices);
                }
                else
                {
                    request.ApproverID = approveRequest.ApproverId;
                    request.Fees = approveRequest.Fees;
                    request.Note = approveRequest.Note;
                    request.File = imageUrl;
                    _context.ApartmentServicesVerifications.Update(request);
                }

                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }
        }
        public async Task<(bool Succeeded, string[] Errors)> ApproveRequestMethod2(ApproveRequestDto approveRequest)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();

            try
            {
                var Request = await GetByIdAsync(approveRequest.RequestId);
               
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });
                }

                var results = await ApproveRequestMethod1(approveRequest);

                if (!results.Succeeded)
                {
                    return (false, results.Errors);
                }

                var currentDate = DateTime.UtcNow;
                var dateAfter3Hours = currentDate.AddHours(3);

                var Approve = new RequestManagement()
                {
                    RequestId = approveRequest.RequestId,
                    ProcedureType = "تعميد",
                    userId = approveRequest.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = "تم تعميد الطلب",
                    ServiceType = (int)ServiceTypeEnum.ApartmentService
                };

                var result = await _RequestManagmentService.AddAsync(Approve);

                if (!result.Succeeded)
                {
                    return (result.Succeeded, result.Errors);
                }

                Request.RequestStuatus = "جارى التعميد";
                Request.IsConfirmation = true;
                var result2 = Update(Request);

                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }
                var ApartmentServicesVerifications = _context.ApartmentServicesVerifications
                   .Include(r => r.Approver)
                   .FirstOrDefault(r => r.RequestId == approveRequest.RequestId && r.IsApproved == false);

                var messageToApprover = MessageSender2.Messages.ApproveRequest(Request.RequestCode);
                var SendMessageResult = await _messageSender.Send3Async(ApartmentServicesVerifications.Approver.PhoneNumber, messageToApprover, null);

                if (!SendMessageResult)
                {
                    _RequestManagmentService.Rollback(trans);
                    return (false, new string[] { "Failed To send Message" });
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

        public async Task<(bool Succeeded, string[] Errors)> AcceptRequest(ReplyDto acceptRequest)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();

            try
            {
                var Request = await GetByIdAsync(acceptRequest.RequestId);
                var ApartmentServicesVerifications = _context.ApartmentServicesVerifications
                    .Include(r => r.Approver)
                    .FirstOrDefault(r => r.RequestId == acceptRequest.RequestId && r.IsApproved == false);

                if (Request == null || ApartmentServicesVerifications==null)
                {
                    return (false, new string[] { "Request Not Found" });
                }
 
                var currentDate = DateTime.UtcNow;
                var dateAfter3Hours = currentDate.AddHours(3);

                var Approve = new RequestManagement()
                {
                    RequestId = acceptRequest.RequestId,
                    ProcedureType = "قبول تعميد",
                    userId = acceptRequest.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = acceptRequest.Message,
                    ServiceType = (int)ServiceTypeEnum.ApartmentService
                };

                var result = await _RequestManagmentService.AddAsync(Approve);

                if (!result.Succeeded)
                {
                    return (result.Succeeded, result.Errors);
                }

                Request.RequestStuatus = "مقبول";
                var result2 = Update(Request);

                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }

                TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
                TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
                ApartmentServicesVerifications.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);
                ApartmentServicesVerifications.IsApproved = true;
                _context.ApartmentServicesVerifications.Update(ApartmentServicesVerifications);
                await _context.SaveChangesAsync();

                var messageToAdmin = MessageSender2.Messages.ApprovedRequest(Request.RequestCode, ApartmentServicesVerifications.Approver.FullName);
                var SendMessageResult = await _messageSender.Send3Async("0536162171", messageToAdmin, null);

                if (!SendMessageResult)
                {
                    _RequestManagmentService.Rollback(trans);
                    return (false, new string[] { "Failed To send Message" });
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
        public  async Task<(bool Succeeded, string[] Errors)> RefuseRequest(ReplyDto acceptRequest)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();

            try
            {
                var Request = await GetByIdAsync(acceptRequest.RequestId);
                var ApartmentServicesVerifications = _context.ApartmentServicesVerifications
                    .Include(r => r.Approver)
                    .FirstOrDefault(r => r.RequestId == acceptRequest.RequestId && r.IsApproved == false);

                if (Request == null || ApartmentServicesVerifications == null)
                {
                    return (false, new string[] { "Request Not Found" });
                }

                var currentDate = DateTime.UtcNow;
                var dateAfter3Hours = currentDate.AddHours(3);

                var Approve = new RequestManagement()
                {
                    RequestId = acceptRequest.RequestId,
                    ProcedureType = "رفض تعميد",
                    userId = acceptRequest.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = acceptRequest.Message,
                    ServiceType = (int)ServiceTypeEnum.ApartmentService
                };

                var result = await _RequestManagmentService.AddAsync(Approve);

                if (!result.Succeeded)
                {
                    return (result.Succeeded, result.Errors);
                }

                Request.RequestStuatus = "مرفوض";
                var result2 = Update(Request);

                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }

                TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
                TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
                ApartmentServicesVerifications.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);
                ApartmentServicesVerifications.IsApproved = false;
                _context.ApartmentServicesVerifications.Update(ApartmentServicesVerifications);
                await _context.SaveChangesAsync();

                var messageToAdmin = MessageSender2.Messages.RefusedRequest(Request.RequestCode, ApartmentServicesVerifications.Approver.FullName);
                var SendMessageResult = await _messageSender.Send3Async("0536162171", messageToAdmin, null);

                if (!SendMessageResult)
                {
                    _RequestManagmentService.Rollback(trans);
                    return (false, new string[] { "Failed To send Message" });
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

        public async Task<(bool Succeeded, string[] Errors)> Delete(int id)
        {
            try
            {
                ApartmentServicesRequest apartmentService = await GetByIdAsync(id);
                if (apartmentService != null)
                {
                    _context.ApartmentServicesRequests.Remove(apartmentService);
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
