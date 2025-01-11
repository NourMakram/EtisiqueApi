using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EcommercePro.Models.Constants;
using System.Runtime.InteropServices;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Stripe;
using Org.BouncyCastle.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EtisiqueApi.Repositiories
{
    public class CommonPartsService : GenaricService<RequestsCommonParts>, ICommonPartsService
    {
        Context _context;
        private IRequestManagement _RequestManagmentService;
        private IAcountService _acountService;
        private ISubServiceRequestService _subServiceRequestService;
        IEmailService _emailService;
        IRequestImage _requestImage;
        MessageSender2.IMessageSender _messageSender; 
        public CommonPartsService(Context context ,IRequestImage requestImage, IRequestManagement requestManagmentService, IAcountService acountService,
            ISubServiceRequestService subServiceRequestService, IEmailService emailService,MessageSender2.IMessageSender messageSender) : base(context)
        {

            _context = context;
            _RequestManagmentService = requestManagmentService;
            _acountService = acountService;
            _subServiceRequestService = subServiceRequestService;
            _emailService = emailService;
            _messageSender=messageSender;
            _requestImage = requestImage;
        }

 
        public RequestsCommonParts LastOrder()
        {

            return _context.RequestCommonParts.AsNoTracking().OrderBy(r => r.CreatedDate).LastOrDefault();

        }
        public IQueryable<RequestsCommonParts> GetRequestToProjectsManager(List<int>Projects,string projectName, int TypeServiceId, string techniciId,
         string ManagerId, string BuildingName, string Status, string Code, bool IsUrget = false, int day = 0
         , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int Note = 0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                .Where(R => Projects.Any(userProject => userProject == R.projectId))
                       .OrderByDescending(R => R.id)
                       .Include(R => R.Manager)
                       .Include(R => R.Technician)
                       .Include(R => R.Project)
                       .Include(R => R.ServiceType)
                       .AsQueryable();
            if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
            else if (TypeServiceId != 0)
            {
                Requests = Requests.Where(R => R.ServiceTypeId == TypeServiceId);
            }
            else if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
            else if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == BuildingName);
            }
            else if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));
            }
            else if (Status != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Status);
            }
            else if (IsUrget == true)
            {
                Requests = Requests.Where(R => R.IsUrgent == IsUrget);

            }
            else if (ManagerId != null)
            {
                Requests = Requests.Where(R => R.ManagerId == ManagerId);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
            else if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= startOfWeek && DateOnly.FromDateTime(R.CreatedDate) < endOfWeek);

            }
            else if (month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                Requests = Requests.Where(R => R.CreatedDate.Month == CurrentMonth);

            }
            else if (year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                Requests = Requests.Where(R => R.CreatedDate.Year == CurrentYear);

            }
            if (from != default)
            {
                Requests = Requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= from);

            }
            if (to != default)
            {
                Requests = Requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) <= to && DateOnly.FromDateTime(R.CreatedDate) >= from);


            }
            if (Note > 0)
            {
                Requests = Requests.Where(R => R.ManagerNote != null);

            }
            return Requests;
        }
        public IQueryable<RequestsCommonParts> FilterRequestsBy(string projectName, int TypeServiceId, string techniciId,
            string ManagerId, string BuildingName, string Status, string Code, bool IsUrget = false, int day = 0
            , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default,int Note=0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                       .OrderByDescending(R => R.id)
                       .Include(R => R.Manager)
                       .Include(R => R.Technician)
                       .Include(R => R.Project)
                       .Include(R => R.ServiceType)
                       .AsQueryable();
            if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
            else if (TypeServiceId != 0)
            {
                Requests = Requests.Where(R => R.ServiceTypeId == TypeServiceId);
            }
            else if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
            else if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == BuildingName);
            }
            else if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));
            }
            else if (Status != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Status);
            }
            else if (IsUrget == true)
            {
                Requests = Requests.Where(R => R.IsUrgent == IsUrget);

            }
            else if(ManagerId != null)
            {
                Requests = Requests.Where(R => R.ManagerId == ManagerId);
            }
            else if(day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
            else if (week > 0)
            {
                 // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(R =>DateOnly.FromDateTime(R.CreatedDate) >= startOfWeek && DateOnly.FromDateTime(R.CreatedDate) < endOfWeek);

            }
            else if (month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                Requests = Requests.Where(R => R.CreatedDate.Month == CurrentMonth);

            }
            else if (year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                Requests = Requests.Where(R =>R.CreatedDate.Year == CurrentYear);

            }
             if (from!=default)
            {
                 Requests = Requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= from);

            }
            if (to != default)
            {
                 Requests = Requests.Where(R =>DateOnly.FromDateTime(R.CreatedDate) <= to && DateOnly.FromDateTime(R.CreatedDate) >= from);

            }
            if(Note> 0)
            {
                Requests = Requests.Where(R =>R.ManagerNote!=null);

            }
            return Requests;
        }
        public CommonPartsResponse Get(int id)
        {
            var SubServices = _subServiceRequestService.GetSubServices(id, (int)ServiceTypeEnum.CommonParts);
            List<string> images = _requestImage.GetImages(id, (int)ServiceTypeEnum.CommonParts);



            return _context.RequestCommonParts.AsNoTracking()
                                    .Include(R => R.Manager)
                                    .Include(R => R.Technician)
                                    .Include(R => R.Project)
                                    .Include(R => R.ServiceType)
                                    .Select(R => new CommonPartsResponse()
                                    {
                                        id = R.id,
                                        RequestCode = R.RequestCode,
                                        RequestImage = R.RequestImage,
                                        RequestStuatus = R.RequestStuatus,
                                        projectName = R.Project.ProjectName,
                                        ManagerName = R.Manager.FullName,
                                        ManagerPhone=R.Manager.PhoneNumber,
                                        TechnicianName = R.Technician.FullName,
                                        TechnicianPhone=R.Technician.PhoneNumber,
                                         SubServices = SubServices,
                                        Description = R.Description,
                                        IsUrgent = R.IsUrgent==true?"تعم":"لا",
                                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        BuildingName = R.BuildingName,
                                        Position = R.Position,
                                        ServiceType = R.ServiceType.Name,
										UnitNo = R.UnitNo,
										Type = R.Type == 1 ? "داخل" : "خارج",
                                        images=images
									}).FirstOrDefault(R => R.id == id);
        }
        public async Task<(bool Succeeded, string[] Errors) >Transfer(TransferDto tranferData)
        {
           
            //get Request
            RequestsCommonParts Request = await GetByIdAsync(tranferData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
           
            var result = await _RequestManagmentService.Transfer(tranferData, (int)ServiceTypeEnum.CommonParts);

            if (!result.Succeeded)
            {

                return (false, result.Errors);

            }
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            Request.RequestStuatus = "جارى المعالجة";
            Request.TechnicianId = tranferData.techiancalId;
            Request.UpdatedDate = dateAfter3Hours;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }

        public async Task<(bool Succeeded, string[] Errors) >Close(CommonPartsmanagmentDto closeData)
        {
            //get Technician
            RequestsCommonParts Request = await GetByIdAsync(closeData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            string Message = $" تم اغلاق الطلب ";

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement Close = new RequestManagement()
            {
                RequestId = closeData.RequestId,
                ProcedureType = "اغلاق",
                userId = closeData.userId,
                CreatedDate = dateAfter3Hours,
                Notes = Message,
                ServiceType = (int)ServiceTypeEnum.CommonParts,
                //Attachments = closeData.filePath


            };
            var result = await RequestManagment(Close);



            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
             

            if (closeData.formFiles != null && closeData.formFiles.Count() > 0)
            {
                var result1 = await _requestImage.Add(closeData.formFiles, Close.Id, "Attchments");
                if (!result1.Succeeded)
                {
                    return (result1.Succeeded, result1.Errors);
                }
            }
            Request.RequestStuatus = "اغلاق";
            TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
            TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
            Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

            Request.UpdatedDate = dateAfter3Hours;
            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);


        }

        //public async Task<(bool Succeeded, string[] Errors)> PartialClose(CommonPartsmanagmentDto PartialCloseData)
        //{
        //    //get Technician
        //    RequestsCommonParts Request = await GetByIdAsync(PartialCloseData.RequestId);
        //    if (Request == null)
        //    {
        //        return (false, new string[] { "Request Not Found" });

        //    }
 
        //    var currentDate = DateTime.UtcNow;

        //    // Increase the hour by 3
        //    var dateAfter3Hours = currentDate.AddHours(3);

        //    RequestManagement PartialClose = new RequestManagement()
        //    {
        //        RequestId = PartialCloseData.RequestId,
        //        ProcedureType = "اقفال",
        //        userId = PartialCloseData.userId,
        //        CreatedDate = dateAfter3Hours,
        //        Notes = Message,
        //        ServiceType = (int)ServiceTypeEnum.CommonParts,
        //        Attachments = PartialCloseData.filePath


        //    };
        //    var result = await RequestManagment(PartialClose);

        //    if (!result.Succeeded)
        //    {
        //        return (false, result.Errors);
        //    }
        //    Request.RequestStuatus = "اقفال جزئى";
        //    Request.UpdatedDate = dateAfter3Hours;

        //    var result2 = Update(Request);

        //    return (result2.Succeeded, result2.Errors);

        //}

        public async Task<(bool Succeeded, string[] Errors)> Comment(CommonPartsmanagmentDto CommentData)
        {
            //get Technician
            RequestsCommonParts Request = await GetByIdAsync(CommentData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            string Message = $" تم تعليق الطلب ";

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement Comment = new RequestManagement()
            {
                RequestId = CommentData.RequestId,
                ProcedureType = "تعليق",
                userId = CommentData.userId,
                CreatedDate = dateAfter3Hours,
                Notes = Message,
                ServiceType = (int)ServiceTypeEnum.CommonParts,
                //Attachments = CommentData.filePath


            };
            var result = await RequestManagment(Comment);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            if (CommentData.formFiles != null && CommentData.formFiles.Count() > 0)
            {
                var result1 = await _requestImage.Add(CommentData.formFiles, Comment.Id, "Attchments");
                if (!result1.Succeeded)
                {
                    return (result1.Succeeded, result1.Errors);
                }
            }
            Request.RequestStuatus = "معلق";
            Request.UpdatedDate = dateAfter3Hours;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);


        }

        public async Task<(bool Succeeded, string[] Errors)> RequestManagment(RequestManagement requestManagement)
        {
            var result = await _RequestManagmentService.AddAsync(requestManagement);

            return (result.Succeeded, result.Errors);

        }

        public IQueryable<RequestsCommonParts> GetTechincanRequest(string techincanId, string stauts, int day = 0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                .Where(R => R.TechnicianId == techincanId || R.ManagerId == techincanId);
            if(stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == stauts);
            }
            else if(day > 0)
            {
              DateOnly  currentData =DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
            return Requests;
        }
        public IQueryable<RequestsCommonParts> GetManagerRequest(string managerId, string stauts,int day=0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                 .Where(R =>  R.ManagerId == managerId);
            if (stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == stauts);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
            return Requests;
        }
      
        public async Task<(bool Succeeded, string[] Errors)> PartialClose(CommonPartsmanagmentDto PartialCloseData1,
            string ProcedureType,string RequestStuatus)
        {
            //get Technician
            RequestsCommonParts Request = await GetByIdAsync(PartialCloseData1.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
 
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement PartialClose = new RequestManagement()
            {
                RequestId = PartialCloseData1.RequestId,
                ProcedureType = ProcedureType,
                userId = PartialCloseData1.userId,
                CreatedDate = dateAfter3Hours,
                Notes = PartialCloseData1.Note,
                ServiceType = (int)ServiceTypeEnum.CommonParts,
               // Attachments = PartialCloseData1.filePath


            };
            var result = await RequestManagment(PartialClose);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            if (PartialCloseData1.formFiles != null && PartialCloseData1.formFiles.Count() > 0)
            {
                var result1 = await _requestImage.Add(PartialCloseData1.formFiles, PartialClose.Id, "Attchments");
                if (!result1.Succeeded)
                {
                    return (result1.Succeeded, result1.Errors);
                }
            }

            Request.RequestStuatus = RequestStuatus;
            Request.UpdatedDate = dateAfter3Hours;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }
        public async Task<(bool Succeeded, string[] Errors)> Confirm(StartDto ConfirmDataDto)
        {
             try
            {
                RequestsCommonParts Request = await GetByIdAsync(ConfirmDataDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                string Message = $"تم اعتماد الطلب";
 
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                RequestManagement Confirm = new RequestManagement()
                {
                    RequestId = ConfirmDataDto.RequestId,
                    ProcedureType = "اعتماد",
                    userId = ConfirmDataDto.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = Message,
                    ServiceType = (int)ServiceTypeEnum.CommonParts,

                };
                var result = await RequestManagment(Confirm);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
  
                Request.RequestStuatus = "اغلاق";
                Request.UpdatedDate = dateAfter3Hours;
                TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
                TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
                Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

                var result2 = Update(Request);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }
                 
 
                return (true, null);
            }
            catch (Exception ex)
            {
 
                return (false, new string[] { ex.Message });
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> Refuse(CloseByNoteDto RefuseDataDto)
        {
            //get Technician
            RequestsCommonParts Request = await GetByIdAsync(RefuseDataDto.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            string Message = $" تم رفض الطلب  ";

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement PartialClose = new RequestManagement()
            {
                RequestId = RefuseDataDto.RequestId,
                ProcedureType = "رفض",
                userId = RefuseDataDto.userId,
                CreatedDate = dateAfter3Hours,
                Notes = RefuseDataDto.Note,
                ServiceType = (int)ServiceTypeEnum.CommonParts,
             };
            var result = await RequestManagment(PartialClose);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            
            Request.RequestStuatus = "مرفوض";
            Request.UpdatedDate = dateAfter3Hours;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }

        public async Task<(bool Succeeded, string[] Errors)> Note(CloseByNoteDto NoteDataDto)
        {
            //get Technician
            RequestsCommonParts Request = await GetByIdAsync(NoteDataDto.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement Note = new RequestManagement()
            {
                RequestId = NoteDataDto.RequestId,
                ProcedureType = "ملاحظة",
                userId = NoteDataDto.userId,
                CreatedDate = dateAfter3Hours,
                Notes = NoteDataDto.Note,
                ServiceType = (int)ServiceTypeEnum.CommonParts,
               // Attachments = NoteDataDto.filePath
            };
            var result = await RequestManagment(Note);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            Request.ManagerNote = NoteDataDto.Note;
            Request.UpdatedDate = dateAfter3Hours;

            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);
        }

        public async Task<(bool Succeeded, string[] Errors)> Up(CommonPartsmanagmentDto UpDataDto)
        {
           var trans =await _RequestManagmentService.BeginTransactionAsync();

            try{
                //get Technician
                RequestsCommonParts Request = await GetByIdAsync(UpDataDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }

                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                RequestManagement PartialClose = new RequestManagement()
                {
                    RequestId = UpDataDto.RequestId,
                    ProcedureType = "تصعيد",
                    userId = UpDataDto.userId,
                    CreatedDate = dateAfter3Hours,
                    Notes = UpDataDto.Note,
                    ServiceType = (int)ServiceTypeEnum.CommonParts,
                    //Attachments = UpDataDto.filePath
                };
                var result = await RequestManagment(PartialClose);


                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                if (UpDataDto.formFiles != null && UpDataDto.formFiles.Count() > 0)
                {
                    var result1 = await _requestImage.Add(UpDataDto.formFiles, PartialClose.Id, "Attchments");
                    if (!result1.Succeeded)
                    {
                        return (result1.Succeeded, result1.Errors);
                    }
                }
                Request.RequestStuatus = "مصعد";
                Request.UpdatedDate = dateAfter3Hours;
                var result2 = Update(Request);

                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }
                var Message = MessageSender2.Messages.Up(Request.RequestCode.ToString());

                var SendMessageResult1 = await _messageSender.Send3Async("0542999881", Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                var SendMessageResult = await _emailService.SendEmailAsync("info@etsaq.com", Message, "تصعيد طلب");
                if (!SendMessageResult.Succeeded)
                {
                    return (SendMessageResult.Succeeded, new string[] { SendMessageResult.MessageError });

                }
                _RequestManagmentService.Commit(trans);
                return (true, null);
            }
            catch(Exception ex)
            {
                _RequestManagmentService.Rollback(trans);
                  return (false, null);

            }
        }
    }
}
