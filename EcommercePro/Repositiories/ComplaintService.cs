using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.InteropServices;
using static EcommercePro.Models.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EtisiqueApi.Repositiories
{
    public class ComplaintService:GenaricService<Complaint> ,IComplaintService
    {
        Context _context;
        private IRequestManagement _RequestManagmentService;
        private readonly IFileService _fileService;
        private IAcountService _acountService;
        private IRequestImage _requestImage;

        public ComplaintService(Context context,IAcountService acountService, IRequestImage requestImage, IRequestManagement requestManagmentService, IFileService fileService) : base(context)
        {

            _context = context;
            _RequestManagmentService = requestManagmentService;
            _fileService = fileService;
            _acountService = acountService;
            _requestImage = requestImage;
        }



        public Complaint LastOrder()
        {

            return _context.Complaints.AsNoTracking().OrderBy(r => r.Date).LastOrDefault();

        }
        public IQueryable<ComplaintDto.ComplaintDtoForShow> FilterRequestsBy(string techniciId = null, int Code = 0,
            string Status = null, string ClientName = null, string ProjectName = null, int day = 0, int week = 0, int Year = 0, int Month = 0)
{
            var Requests = _context.Complaints.AsNoTracking()
                       .OrderByDescending(R => R.Id)
                        .Include(R => R.TransferredTo)
                       .Include(R => R.Project)
                       .Include(R => R.Client)
                       .Include(R => R.CreatedBy)
                       .AsQueryable();
            if (ProjectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(ProjectName));
            }
             
            else if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TransferredToId == techniciId);
            }
            else if (ClientName != null)
            {
                Requests = Requests.Where(R => R.Client.FullName == ClientName);
            }
            else if (Code != 0)
            {
                Requests = Requests.Where(R => R.Number.ToString().Contains(Code.ToString()));
            }
            else if (Status != null)
            {
                Requests = Requests.Where(R => R.Status == Status);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.Status == "جديد" && DateOnly.FromDateTime(R.Date) == currentData);

            }
            if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(request => DateOnly.FromDateTime(request.Date) >= startOfWeek && DateOnly.FromDateTime(request.Date) < endOfWeek);

            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;
                Requests = Requests.Where(Q => Q.Date.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                Requests = Requests.Where(Q => Q.Date.Month == CurrentMonth);

            }
            return Requests.Select(c=>new ComplaintDto.ComplaintDtoForShow()
            {
                Id=c.Id,
                BuildingCode=c.BuildingCode,
                UnitNo=c.UnitNo,
                ProjectName=c.Project.ProjectName,
                ClientName=c.Client.FullName,
                ClientPhone=c.Client.PhoneNumber,
                TechincalName=c.TransferredTo.FullName,
                TechincalPhone=c.TransferredTo.PhoneNumber,
                ComplaintIssueFile=c.ComplaintIssueFile,
                Date=c.Date.ToString("dd-MM-yyyy"),
                IssueDetails=c.IssueDetails,
                Stuats=c.Status,
                CreatedBy=c.CreatedBy.FullName,
                Code=c.Number.ToString()

            });
        }
        public ComplaintDto.ComplaintDtoForShow Get(int id)
        {
 


            return _context.Complaints.AsNoTracking()
                                    .Include(R => R.TransferredTo)
                                    .Include(R => R.Project)
                                    .Include(R => R.Client)
                                    .Include(R => R.CreatedBy)
                                    .Select(c => new ComplaintDto.ComplaintDtoForShow()
                                    {
                                        Id = c.Id,
                                        BuildingCode = c.BuildingCode,
                                        UnitNo = c.UnitNo,
                                        ProjectName = c.Project.ProjectName,
                                        ClientName = c.Client.FullName,
                                        ClientPhone = c.Client.PhoneNumber,
                                        TechincalName = c.TransferredTo.FullName,
                                        TechincalPhone = c.TransferredTo.PhoneNumber,
                                        ComplaintIssueFile = c.ComplaintIssueFile,
                                        Date = c.Date.ToString("dd-MM-yyyy"),
                                        IssueDetails = c.IssueDetails,
                                        Stuats = c.Status,
                                        CreatedBy = c.CreatedBy.FullName,
                                        Code = c.Number.ToString()

                                    }).FirstOrDefault(R => R.Id == id);
        }


        public async Task<(bool Succeeded, string[] Errors)> Transfer(TransferDto tranferData)
        {

            //get Request
            Complaint Request = await GetByIdAsync(tranferData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }

            var result = await _RequestManagmentService.Transfer(tranferData, (int)ServiceTypeEnum.ComplaintService);

            if (!result.Succeeded)
            {

                return (false, result.Errors);

            }
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            Request.Status = "جارى المعالجة";
            Request.TransferredToId = tranferData.techiancalId;
 
            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }

        public async Task<(bool Succeeded, string[] Errors)> Close(CommonPartsmanagmentDto closeData)
        {
            //if (closeData.file != null)
            //{
            //    var resultt = await _fileService.SaveImage("complaints", closeData.file);
            //    closeData.filePath = resultt.imagePath;

            //}
            //get Technician
            Complaint Request = await GetByIdAsync(closeData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            string Message = $" تم اغلاق الشكوة ";

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
                ServiceType = (int)ServiceTypeEnum.ComplaintService,
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
            Request.Status = "اغلاق";
            var resultDate = (dateAfter3Hours - Request.Date);
             var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);


        }

        public async Task<(bool Succeeded, string[] Errors)> PartialClose(CommonPartsmanagmentDto PartialCloseData)
        {
            //if (PartialCloseData.file != null)
            //{
            //    var resultt = await _fileService.SaveImage("complaints", PartialCloseData.file);
            //    PartialCloseData.filePath = resultt.imagePath;

            //}
            //get Technician
            Complaint Request = await GetByIdAsync(PartialCloseData.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            string Message = $" تم اقفال الشكوة ";

            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);

            RequestManagement PartialClose = new RequestManagement()
            {
                RequestId = PartialCloseData.RequestId,
                ProcedureType = "اقفال",
                userId = PartialCloseData.userId,
                CreatedDate = dateAfter3Hours,
                Notes = Message,
                ServiceType = (int)ServiceTypeEnum.ComplaintService,
                //Attachments = PartialCloseData.filePath


            };
            var result = await RequestManagment(PartialClose);

            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
             

            if (PartialCloseData.formFiles != null && PartialCloseData.formFiles.Count() > 0)
            {
                var result1 = await _requestImage.Add(PartialCloseData.formFiles, PartialClose.Id, "Attchments");
                if (!result1.Succeeded)
                {
                    return (result1.Succeeded, result1.Errors);
                }
            }
            Request.Status = "اقفال جزئى";
 
            var result2 = Update(Request);

            return (result2.Succeeded, result2.Errors);

        }



        public async Task<(bool Succeeded, string[] Errors)> RequestManagment(RequestManagement requestManagement)
        {
            var result = await _RequestManagmentService.AddAsync(requestManagement);

            return (result.Succeeded, result.Errors);

        }

        public IQueryable<ComplaintDto.ComplaintDtoForShow> GetTechincanRequest(string techincanId, string stauts, int day = 0)
        {
            var Requests = _context.Complaints.AsNoTracking()
                .Where(R => R.TransferredToId == techincanId || R.CreatedById==techincanId)
                .OrderByDescending(R => R.Id).AsQueryable();
            ;
            if (stauts != null)
            {
                Requests = Requests.Where(R => R.Status == stauts);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.Status == "جديد" && DateOnly.FromDateTime(R.Date) == currentData);

            }
            return Requests.Select(c=>new ComplaintDto.ComplaintDtoForShow()
            {
                Id = c.Id,
                BuildingCode = c.BuildingCode,
                UnitNo = c.UnitNo,
                ProjectName = c.Project.ProjectName,
                ClientName = c.Client.FullName,
                ClientPhone = c.Client.PhoneNumber,
                TechincalName = c.TransferredTo.FullName,
                TechincalPhone = c.TransferredTo.PhoneNumber,
                ComplaintIssueFile = c.ComplaintIssueFile,
                Date = c.Date.ToString("dd-MM-yyyy"),
                IssueDetails = c.IssueDetails,
                Stuats = c.Status,
                CreatedBy = c.CreatedBy.FullName,
                Code = c.Number.ToString()

            }).AsQueryable();
        }

        public  async Task<(bool Succeeded, string[] Errors)> Add(ComplaintDto.ComplaintDtoForAdd complaintDto)
        {
            if (complaintDto.formFile != null)
            {
                var resultt = await _fileService.SaveImage("complaints", complaintDto.formFile);
                complaintDto.ComplaintIssueFile = resultt.imagePath;

            }
            Complaint lastOrder = LastOrder();
            int RequestCode = 1100;

            if (lastOrder != null)
            {
                RequestCode = (lastOrder.Number + 1);

            }
            Complaint Request = new Complaint()
            {
                 
                ComplaintIssueFile = complaintDto?.ComplaintIssueFile,
                Status = "جديد",
                ProjectId = complaintDto.ProjectId,
                BuildingCode = complaintDto.BuildingCode,
                UnitNo = complaintDto.UnitNo,
                ClientId=complaintDto.ClientId,
                CreatedById=complaintDto.CreatedById,
                Number=RequestCode,
                IssueDetails=complaintDto.IssueDetails,
                
                
            };
            var result = await AddAsync(Request);

            return (result.Succeeded, result.Errors);



        }

        public async Task<(bool Succeeded, string[] Errors)> Delete(int Id)
        {
            //get Technician
            Complaint Request = await GetByIdAsync(Id);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            var result = Delete(Request);
            if (result.Succeeded)
            {
                return (true, null);
            }
            return (result.Succeeded, result.Errors);



        }

        public IQueryable<ComplaintDto.ComplaintDtoForShow> GetRequestToProjectsManager(List<int> projects, string techniciId = null, int Code = 0,
            string Status = null, string ClientName = null, string ProjectName = null, int day = 0, int week = 0, int Year = 0, int Month = 0)
        {
            //List<int> projects = _acountService.GetUserProjects(UserId);

             
            var Requests = _context.Complaints.AsNoTracking()
                        .Include(R => R.TransferredTo)
                        .Include(R => R.Project)
                        .Include(R => R.Client)
                        .Include(R => R.CreatedBy)
                        .Where(R => projects.Any(userProject => userProject == R.ProjectId))
                        .OrderByDescending(R=>R.Id)
                        .AsQueryable();

            if (ProjectName != null)
            {
                Requests = Requests.Where(R => R.Project.ProjectName.Contains(ProjectName));
            }

            else if (techniciId != null)
            {
                Requests = Requests.Where(R => R.TransferredToId == techniciId);
            }

            else if (Code != null)
            {
                Requests = Requests.Where(R => R.Number.ToString().Contains(Code.ToString()));
            }
            else if (Status != null)
            {
                Requests = Requests.Where(R => R.Status == Status);
            }
            else if (day > 0)
            {
                DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
                Requests = Requests.Where(R => R.Status == "جديد" && DateOnly.FromDateTime(R.Date) == currentData);

            }
            if (week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                Requests = Requests.Where(request => DateOnly.FromDateTime(request.Date) >= startOfWeek && DateOnly.FromDateTime(request.Date) < endOfWeek);

            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;
                Requests = Requests.Where(Q => Q.Date.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;
                Requests = Requests.Where(Q => Q.Date.Month == CurrentMonth);

            }
            return Requests.Select(c => new ComplaintDto.ComplaintDtoForShow()
            {
                Id = c.Id,
                BuildingCode = c.BuildingCode,
                UnitNo = c.UnitNo,
                ProjectName = c.Project.ProjectName,
                ClientName = c.Client.FullName,
                ClientPhone = c.Client.PhoneNumber,
                TechincalName = c.TransferredTo.FullName,
                TechincalPhone = c.TransferredTo.PhoneNumber,
                ComplaintIssueFile = c.ComplaintIssueFile,
                Date = c.Date.ToString("dd-MM-yyyy"),
                IssueDetails = c.IssueDetails,
                Stuats = c.Status,
                CreatedBy = c.CreatedBy.FullName,
                Code = c.Number.ToString()

            });
        }
    }
}
