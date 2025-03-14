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
        private IFileService _fileService;
        public CommonPartsService(Context context, IFileService fileService ,IRequestImage requestImage, IRequestManagement requestManagmentService, IAcountService acountService,
            ISubServiceRequestService subServiceRequestService, IEmailService emailService,MessageSender2.IMessageSender messageSender) : base(context)
        {

            _context = context;
            _RequestManagmentService = requestManagmentService;
            _acountService = acountService;
            _subServiceRequestService = subServiceRequestService;
            _emailService = emailService;
            _messageSender=messageSender;
            _requestImage = requestImage;
            _fileService = fileService;
        }

 
        public RequestsCommonParts LastOrder(bool IsCleaning)
        {

            return _context.RequestCommonParts.AsNoTracking().OrderBy(r => r.CreatedDate).LastOrDefault(p=>p.IsCleaning== IsCleaning);

        }
        public IQueryable<RequestsCommonParts> GetRequestToProjectsManager(string userId ,List<int>Projects, bool IsCleaning,string projectName, int TypeServiceId, string techniciId,
         string ManagerId, string BuildingName, string Status, string Code, bool IsUrget = false, int day = 0
         , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int ConfRequest = 0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                .Where(R => Projects.Any(userProject => userProject == R.projectId) && R.IsCleaning == IsCleaning)
				      .OrderByDescending(R => R.UpdatedDate)
					   .Include(R => R.Manager)
                       .Include(R => R.Technician)
                       .Include(R => R.Project)
                       .Include(R => R.ServiceType)
                       .AsQueryable();
            if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
             if (TypeServiceId != 0)
            {
                Requests = Requests.Where(R => R.ServiceTypeId == TypeServiceId);
            }
             if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
             if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == BuildingName);
            }
             if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));
            }
             if (Status != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Status);
            }
             if (IsUrget == true)
            {
                Requests = Requests.Where(R => R.IsUrgent == IsUrget);

            }
              if (ManagerId != null)
            {
                Requests = Requests.Where(R => R.ManagerId == ManagerId);
            }
              if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.UtcNow);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
             if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.UtcNow.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= startOfWeek && DateOnly.FromDateTime(R.CreatedDate) < endOfWeek);

            }
             if (month > 0)
            {
                int CurrentMonth = DateTime.UtcNow.Month;
                Requests = Requests.Where(R => R.CreatedDate.Month == CurrentMonth);

            }
             if (year > 0)
            {
                int CurrentYear = DateTime.UtcNow.Year;

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
			if (ConfRequest != 0)
			{
				if (ConfRequest == 1)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "جارى التعميد" && R.CommonPartsVerifications.ApproverID == userId);

				}
				if (ConfRequest == 2)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "مقبول" && R.IsConfirmation == true && R.CommonPartsVerifications.ApproverID == userId);

				}
				if (ConfRequest == 3)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "مرفوض" && R.IsConfirmation == true && R.CommonPartsVerifications.ApproverID == userId);

				}
			}
		
			return Requests;
        }
        public IQueryable<RequestsCommonParts> FilterRequestsBy(bool IsCleaning, string projectName, int TypeServiceId, string techniciId,
            string ManagerId, string BuildingName, string Status, string Code, bool IsUrget = false, int day = 0
            , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int ConfRequest = 0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                       .OrderByDescending(R => R.UpdatedDate)
					   .Where(R=>R.IsCleaning == IsCleaning)
                       .Include(R => R.Manager)
                       .Include(R => R.Technician)
                       .Include(R => R.Project)
                       .Include(R => R.ServiceType)
                       .AsQueryable();
            if (projectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
             if (TypeServiceId != 0)
            {
                Requests = Requests.Where(R => R.ServiceTypeId == TypeServiceId);
            }
             if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TechnicianId == techniciId);
            }
             if (BuildingName != null)
            {
                Requests = Requests.Where(R => R.BuildingName == BuildingName);
            }
             if (Code != null)
            {
                Requests = Requests.Where(R => R.RequestCode.ToString().Contains(Code));
            }
             if (Status != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == Status);
            }
             if (IsUrget == true)
            {
                Requests = Requests.Where(R => R.IsUrgent == IsUrget);

            }
             if(ManagerId != null)
            {
                Requests = Requests.Where(R => R.ManagerId == ManagerId);
            }
             if(day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.UtcNow);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
             if (week > 0)
            {
                 // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.UtcNow.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(R =>DateOnly.FromDateTime(R.CreatedDate) >= startOfWeek && DateOnly.FromDateTime(R.CreatedDate) < endOfWeek);

            }
             if (month > 0)
            {
                int CurrentMonth = DateTime.UtcNow.Month;
                Requests = Requests.Where(R => R.CreatedDate.Month == CurrentMonth);

            }
             if (year > 0)
            {
                int CurrentYear = DateTime.UtcNow.Year;

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
			if (ConfRequest != 0)
			{
				if (ConfRequest == 1)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "جارى التعميد");

				}
				if (ConfRequest == 2)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "مقبول" && R.IsConfirmation == true);

				}
				if (ConfRequest == 3)
				{
					Requests = Requests.Where(R => R.RequestStuatus == "مرفوض" && R.IsConfirmation == true);

				}
			}
			return Requests;
        }
        public CommonPartsResponse Get(int id)
        {
            var SubServices = _subServiceRequestService.GetSubServices(id, (int)ServiceTypeEnum.CommonParts);
            List<string> images = _requestImage.GetImages(id, (int)ServiceTypeEnum.CommonParts);

			var RequestVerivication = _context.CommonPartsVerifications.Include(R => R.Approver)
			   .Where(R => R.RequestId == id)
			   .Select(R => new ServicesVerificationsDto()
			   {
				   ApproverName = R.Approver.FullName,
				   IsApproved = R.IsApproved,
				   ApproverId = R.ApproverID,
				   Fees = R.Fees,
				   File = R.File,
				   Note = R.Note,
				   TimeElapsed = R.TimeElapsed
			   }).FirstOrDefault();


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
                                        IsUrgent = R.IsUrgent==true?"نعم":"لا",
                                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                        BuildingName = R.BuildingName,
                                        Position = R.Position,
                                        ServiceType = R.ServiceType.Name,
										UnitNo = R.UnitNo,
										Type = R.Type == 1 ? "داخل" : "خارج",
                                        images=images,
                                        servicesVerification= RequestVerivication,
                                        timeElasped = R.TimeElapsed,
                                        IsCleaning=R.IsCleaning

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
            TimeSpan resultDate = (dateAfter3Hours - Request.UpdatedDate);
            TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
            Request.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

            Request.UpdatedDate = dateAfter3Hours;
            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);


        }


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
                Notes = CommentData?.Note !=null ? CommentData?.Note:Message,
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

        public IQueryable<RequestsCommonParts> GetTechincanRequest(bool IsCleaning, string techincanId, string stauts, int day = 0)
        {
            var Requests = _context.RequestCommonParts.OrderByDescending(R => R.UpdatedDate).AsNoTracking()
                .Where(R => R.TechnicianId == techincanId || R.ManagerId == techincanId && R.IsCleaning == IsCleaning);
            if(stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == stauts);
            }
            else if(day > 0)
            {
              DateOnly  currentData =DateOnly.FromDateTime(DateTime.UtcNow);
                Requests = Requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);

            }
            return Requests;
        }
        public IQueryable<RequestsCommonParts> GetManagerRequest(bool IsCleaning, string managerId, string stauts,int day=0)
        {
            var Requests = _context.RequestCommonParts.AsNoTracking()
                 .Where(R =>  R.ManagerId == managerId && R.IsCleaning == IsCleaning);
            if (stauts != null)
            {
                Requests = Requests.Where(R => R.RequestStuatus == stauts);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.UtcNow);
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

           
            if(ProcedureType=="اقفال اول")
            {
				TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
				TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
				Request.TimeElapsedClose1 = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

            }
            if(ProcedureType=="اقفال ثانى")
            {
				TimeSpan resultDate = (dateAfter3Hours - Request.UpdatedDate);
				TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
				Request.TimeElapsedClose2 = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);

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
                TimeSpan resultDate = (dateAfter3Hours - Request.UpdatedDate);
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

                var SendMessageResult1 = await _messageSender.Send3Async("0536162171", Message, null);
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
					Notes ="تم ارسال الطلب للتعميد",
					ServiceType = (int)ServiceTypeEnum.CommonParts
				};

				var result = await _RequestManagmentService.AddAsync(Approve);

				if (!result.Succeeded)
				{
					return (result.Succeeded, result.Errors);
				}

				Request.RequestStuatus = "جارى التعميد";
				Request.IsConfirmation = true;
				Request.UpdatedDate = dateAfter3Hours;
				var result2 = Update(Request);

				if (!result2.Succeeded)
				{
					return (false, result2.Errors);
				}
                var CommonPartsVerifications = _context.CommonPartsVerifications
                    .Include(r => r.Approver)
                    .FirstOrDefault(r => r.RequestId == approveRequest.RequestId && r.IsApproved == false);

                var messageToApprover = MessageSender2.Messages.ApproveRequest(Request.RequestCode);
                var SendMessageResult = await _messageSender.Send3Async(CommonPartsVerifications.Approver.PhoneNumber, messageToApprover, null);

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

		public  async Task<(bool Succeeded, string[] Errors)> ApproveRequestMethod1(ApproveRequestDto approveRequest)
		{
			try
			{
				var request = _context.CommonPartsVerifications.FirstOrDefault(r => r.RequestId == approveRequest.RequestId && r.IsApproved == false);
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
					var commonpartsServices = new CommonPartsVerifications
					{
						ApproverID = approveRequest.ApproverId,
						RequestId = approveRequest.RequestId,
						Fees = approveRequest.Fees,
						Note = approveRequest.Note,
						IsApproved = false,
						File = imageUrl
					};

					_context.CommonPartsVerifications.Add(commonpartsServices);
				}
				else
				{
					request.ApproverID = approveRequest.ApproverId;
					request.Fees = approveRequest.Fees;
					request.Note = approveRequest.Note;
					request.File = imageUrl;
					_context.CommonPartsVerifications.Update(request);
				}

				await _context.SaveChangesAsync();

				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, new string[] { ex.Message });
			}
		}

		public async Task<(bool Succeeded, string[] Errors)> AcceptRequest(ReplyDto acceptRequest)
		{
			var trans = await _RequestManagmentService.BeginTransactionAsync();

			try
			{
				var Request = await GetByIdAsync(acceptRequest.RequestId);
				var CommonPartsVerifications = _context.CommonPartsVerifications
					.Include(r => r.Approver)
					.FirstOrDefault(r => r.RequestId == acceptRequest.RequestId && r.IsApproved == false);

				if (Request == null || CommonPartsVerifications == null)
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
					ServiceType = (int)ServiceTypeEnum.CommonParts
				};

				var result = await _RequestManagmentService.AddAsync(Approve);

				if (!result.Succeeded)
				{
					return (result.Succeeded, result.Errors);
				}

				Request.RequestStuatus = "مقبول";
				Request.UpdatedDate = dateAfter3Hours;
				var result2 = Update(Request);

				if (!result2.Succeeded)
				{
					return (false, result2.Errors);
				}

				TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
				TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
				CommonPartsVerifications.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);
				CommonPartsVerifications.IsApproved = true;
				_context.CommonPartsVerifications.Update(CommonPartsVerifications);
				await _context.SaveChangesAsync();

				var messageToAdmin = MessageSender2.Messages.ApprovedRequest(Request.RequestCode, CommonPartsVerifications.Approver.FullName);
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

		public async Task<(bool Succeeded, string[] Errors)> RefuseRequest(ReplyDto acceptRequest)
		{
			var trans = await _RequestManagmentService.BeginTransactionAsync();

			try
			{
				var Request = await GetByIdAsync(acceptRequest.RequestId);
				var CommonPartsVerifications = _context.CommonPartsVerifications
					.Include(r => r.Approver)
					.FirstOrDefault(r => r.RequestId == acceptRequest.RequestId && r.IsApproved == false);

				if (Request == null || CommonPartsVerifications == null)
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
					ServiceType = (int)ServiceTypeEnum.CommonParts
				};

				var result = await _RequestManagmentService.AddAsync(Approve);

				if (!result.Succeeded)
				{
					return (result.Succeeded, result.Errors);
				}

				Request.RequestStuatus = "مرفوض";
                Request.UpdatedDate = dateAfter3Hours;
				var result2 = Update(Request);

				if (!result2.Succeeded)
				{
					return (false, result2.Errors);
				}

				TimeSpan resultDate = (dateAfter3Hours - Request.DateOfVisit);
				TimeSpan timeElapsed = new TimeSpan(resultDate.Days, resultDate.Hours, resultDate.Minutes, 0);
				CommonPartsVerifications.TimeElapsed = string.Format("{0}:{1:D2}:{2:D2}", timeElapsed.Days, timeElapsed.Hours, timeElapsed.Minutes);
				CommonPartsVerifications.IsApproved = false;
				_context.CommonPartsVerifications.Update(CommonPartsVerifications);
				await _context.SaveChangesAsync();

				var messageToAdmin = MessageSender2.Messages.RefusedRequest(Request.RequestCode, CommonPartsVerifications.Approver.FullName);
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
                RequestsCommonParts RequestsCommonParts = await GetByIdAsync(id);
                if (RequestsCommonParts != null)
                {
                    _context.RequestCommonParts.Remove(RequestsCommonParts);
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
        public async Task<(bool Succeeded, string[] Errors)> ChangeData()
        {
            IQueryable<RequestsCommonParts> commonParts = _context.RequestCommonParts.Where(R => R.RequestStuatus == "اقفال جزئى");
            foreach (RequestsCommonParts commonParts1 in commonParts)
            {
                commonParts1.RequestStuatus = "اقفال اول";
            }
            _context.RequestCommonParts.UpdateRange(commonParts);
           await  _context.SaveChangesAsync();

            return (true,null);

        }

        public async Task<bool> AreTwentyRequestsAddedTodayAsync(DateOnly date)
        {
            
            var count = await _context.RequestCommonParts.Where(R=>R.IsCleaning == false)
                .CountAsync(r => DateOnly.FromDateTime(r.DateOfVisit) == date); // تأكد من أن لديك حقل DateOfVisit

            //return count >= 2;
            return count >= 20; // إرجاع true إذا كان العدد 20 أو أكثر

        }

    }
}
