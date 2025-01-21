using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EcommercePro.Models.Constants;
using static EtisiqueApi.DTO.EmergencyRequestDto;

namespace EtisiqueApi.Repositiories
{
    public class EmergencyService : GenaricService<Models.Emergency>, IEmergencyService
    {
        private readonly Context _context;
        private IAcountService _acountService;
        private IRequestManagement _RequestManagmentService;
        private IEmailService _emailService;
        private MessageSender2.IMessageSender _messageSender;
        private IShortCutService _shortCutService;
        private ICustomerService _customerService;
        private IFileService _fileService;
        private IRequestImage _requestImage;
        public EmergencyService(Context context, IAcountService acountService, IEmailService emailService,
            IRequestManagement RequestManagmentService, IFileService fileService,
            MessageSender2.IMessageSender messageSender,IRequestImage requestImage,
            IShortCutService shortCutService, ICustomerService customerService) : base(context)
        {

            _context = context;
            _acountService = acountService;
            _RequestManagmentService = RequestManagmentService;
            _emailService = emailService;
            _messageSender = messageSender;
            _shortCutService = shortCutService;
            _customerService = customerService;
            _fileService = fileService;
            _requestImage = requestImage;



        }

        public async Task<(bool Succeeded, string[] Errors)> Add([FromForm]EmergencyRequestDto.EmergencyRequestToAdd emergencyRequestToAdd)
        {

            var trans = await _RequestManagmentService.BeginTransactionAsync();

            try
            {
               
                var customerId = emergencyRequestToAdd.CustomerId;
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                Models.Emergency lastOrder1 = LastOrder();
                int RequestCode = 1100;

                if (lastOrder1 != null)
                {
                    RequestCode = (lastOrder1.RequestCode + 1);

                }
                //genetate Code
                Random generator = new Random();
                string CloseCode = generator.Next(0, 10000).ToString("D4");

                if (emergencyRequestToAdd.CustomerId == 0)
                {
                    RegisterDto newUser = new RegisterDto()
                    {
                        FullName = emergencyRequestToAdd.ClientName,
                        Email = emergencyRequestToAdd.Email,
                        PhoneNumber = emergencyRequestToAdd.ClientPhone,
                        projectId = emergencyRequestToAdd.projectId,
                        UnitNo = emergencyRequestToAdd.UnitNo,
                        BulidingName = emergencyRequestToAdd.BuildingName,
                        City = emergencyRequestToAdd.Address,
                        Latitude = emergencyRequestToAdd.Latitude,
                        Longitude = emergencyRequestToAdd.Longitude,
                        Password = "Etsaq@2000",
                        TypeProject = "مالك"
                    };
                    var ResultData = await _acountService.AddNewClient(newUser);

                    if (!ResultData.Succeeded)
                    {
                        return (false, ResultData.Errors);
                    }
                    customerId = ResultData.id;
                }
                Customer customerdb = await _customerService.GetByIdAsync(customerId);
                if (customerdb == null)
                {
                    return (false, new string[] { "Customer Not Found" });
                }
               Models.Emergency Request = new Models.Emergency()
                {
                    CreatedDate = dateAfter3Hours,
                    Description = emergencyRequestToAdd.Description,
                    CustomerId = customerdb.Id,
                    RequestCode = RequestCode,
                    CloseCode = CloseCode,
                    RequestStuatus = "جديد",
                    projectId = emergencyRequestToAdd.projectId,
                    BuildingName = emergencyRequestToAdd.BuildingName,
                    UnitNo = emergencyRequestToAdd.UnitNo,
                    EmergencyId=emergencyRequestToAdd.EmergencyId
                };
                var result2 = await AddAsync(Request);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }

                if (emergencyRequestToAdd.formFiles != null && emergencyRequestToAdd.formFiles.Count() > 0)
                {
                   var resultt = await _requestImage.Add(emergencyRequestToAdd.formFiles, (int)ServiceTypeEnum.emergencyRequest, Request.id, "Emergencies");
                    if (!resultt.Succeeded)
                    {
                        return (false, resultt.Errors);
                    }

                }
                var Resut33 = await SendMessages(RequestCode, CloseCode, customerdb.ApplicationUser.PhoneNumber);
                if (!Resut33.Succeeded)
                {
                    return (false, Resut33.Errors);
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
        

        public async Task<(bool Succeeded, string[] Errors)> SendMessages(int RequestCode, string closeCode, string clientPhone)
        {


            var Message1 = MessageSender2.Messages.NewEmergencyMsg(RequestCode, closeCode);

            var SendMessageResult = await _messageSender.Send3Async(clientPhone, Message1, null);
            if (!SendMessageResult)
            {
                return (false, new string[] { "Faild To send Message" });

            }
            var Message2 = MessageSender2.Messages.NewEmergencyMsgToAdmin(RequestCode);

            var SendMessageResult1 = await _messageSender.Send3Async("0536162171", Message2, null);
            if (!SendMessageResult1)
            {
                return (false, new string[] { "Faild To send Message" });

            }
            return (true, null);
            //var results = await _emailService.SendEmailAsync(customerdb.ApplicationUser.Email, Message, "اتساق");
            //if (!results.Succeeded)
            //{
            //    return (false, new string[] { results.MessageError });

            //}


        }


        public Models.Emergency LastOrder()
        {
            return _context.Emergencies.AsNoTracking().OrderBy(r => r.CreatedDate).LastOrDefault();
        }

        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> FilterRequestsBy(string projectName = null, 
            int Type = 0, string techniciId = null, string ClientName = null, string BuildingName = null, 
            int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day = 0,
            int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default)
        {
            var requests = _context.Emergencies.AsNoTracking()
                     .Include(R => R.Project)
                     .Include(R=>R.EmergencyTypes)
                     .Include(R => R.Customer)
                     .ThenInclude(R => R.ApplicationUser)
                     .OrderByDescending(R => R.id)
                     .AsQueryable();
            if (projectName != null)
            {
                requests = requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
            if (Type != 0)
            {
                requests = requests.Where(R => R.EmergencyId == Type);

            }
            if (techniciId != null)
            {
                requests = requests.Where(R => R.TechnicianId == techniciId);

            }
            if (ClientName != null)
            {
                requests = requests.Where(R => R.Customer.ApplicationUser.FullName.Contains(ClientName));

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
                requests = requests.Where(R => R.ApplicationUser.PhoneNumber.Contains(ClientPhone));

            }
            //else if (day > 0)
            //{
            //    DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
            //    requests = requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.CreatedDate) == currentData);


            //}
            //else if (week > 0)
            //{
            //    // تحديد بداية ونهاية الأسبوع الحالي
            //    DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
            //    DateOnly endOfWeek = startOfWeek.AddDays(7);

            //    // فلترة الطلبات خلال هذا الأسبوع
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= startOfWeek && DateOnly.FromDateTime(R.CreatedDate) < endOfWeek);

            //}
            //else if (month > 0)
            //{
            //    int CurrentMonth = DateTime.Now.Month;
            //    requests = requests.Where(R => R.CreatedDate.Month == CurrentMonth);

            //}
            //else if (year > 0)
            //{
            //    int CurrentYear = DateTime.Now.Year;

            //    requests = requests.Where(R => R.CreatedDate.Year == CurrentYear);

            //}
            //if (from != default)
            //{
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) >= from);

            //}
            //if (to != default)
            //{
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.CreatedDate) <= to && DateOnly.FromDateTime(R.CreatedDate) >= from);


            //}
 
            return requests
                    .Select(R => new EmergencyRequestDto.EmergencyRequestToShow()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        ClientName = R.Customer.ApplicationUser.FullName,
                        ClientPhone = R.Customer.ApplicationUser.PhoneNumber,
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Customer.City,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                         RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                         TimeElapsed = R.TimeElapsed.ToString(),
                        EmergencyType = R.EmergencyTypes.Name,
                    }).AsQueryable(); ;

        }

        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToCustomer(int CustomerId, string code, string Status, DateOnly date = default)
        {
            IQueryable<Models.Emergency> requests =
                _context.Emergencies.Include(R => R.Project)
                    .Include(R => R.Customer)
                    .ThenInclude(R => R.ApplicationUser)
                    .Include(R => R.EmergencyTypes)
                    .Include(R => R.ApplicationUser)
                     .OrderByDescending(R => R.id).AsQueryable();
            if (code != null && date != default)
            {
                requests = requests.AsNoTracking()
                               .Where(R => R.CustomerId == CustomerId &&
                               R.RequestCode.ToString().Contains(code)
                               && (R.CreatedDate.Day == date.Day && R.CreatedDate.Month == date.Month && R.CreatedDate.Year == date.Year))
                               .AsQueryable();

            }
            else if (code != null)
            {
                requests = requests.AsNoTracking()
                            .Where(R => R.CustomerId == CustomerId && R.RequestCode.ToString().Contains(code))
                            .AsQueryable();


            }
            else if (date != default)
            {
                requests = requests.AsNoTracking()
                            .Where(R => R.CustomerId == CustomerId &&
                            (date != default && (R.CreatedDate.Day == date.Day && R.CreatedDate.Month ==
                            date.Month && R.CreatedDate.Year == date.Year)))
                            .AsQueryable();

            }
            else if (Status != null)
            {
                if (Status == "0")
                {
                    requests = requests.AsNoTracking()
                            .Where(R => R.CustomerId == CustomerId && R.RequestStuatus != "اغلاق")
                            .AsQueryable();
                }
                else
                {
                    requests = requests.AsNoTracking()
                                                .Where(R => R.CustomerId == CustomerId && R.RequestStuatus == "اغلاق")
                                                .AsQueryable();
                }
            }
            else
            {
                requests = requests.AsNoTracking()
                            .Where(R => R.CustomerId == CustomerId)
                            .AsQueryable();

            }
            return requests
                    .Select(R => new EmergencyRequestToShow()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        ClientName = R.Customer.ApplicationUser.FullName,
                        ClientPhone = R.Customer.ApplicationUser.PhoneNumber,
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Customer.City,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        TimeElapsed = R.TimeElapsed.ToString(),
                        EmergencyType = R.EmergencyTypes.Name,
                        CloseCode = R.CloseCode

,
                    }).AsQueryable();
        }

        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToTechninics(string TechincanId, string Status, int day)
        {
            var requests = _context.Emergencies.AsNoTracking()
                            .Where(R => R.TechnicianId == TechincanId)
                            .AsQueryable();
            if (Status != null)
            {
                requests = requests
                          .Where(R => R.RequestStuatus == Status)
                            .AsQueryable();
            }
            return requests
                    .Include(R => R.Project)
                    .Include(R => R.Customer)
                    .ThenInclude(R=>R.ApplicationUser)
                    .Include(R => R.EmergencyTypes)
                    .Include(R => R.ApplicationUser)
                     .OrderByDescending(R => R.id)
                    .Select(R => new EmergencyRequestToShow()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        ClientName = R.Customer.ApplicationUser.FullName,
                        ClientPhone = R.Customer.ApplicationUser.PhoneNumber,
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Customer.City,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        TimeElapsed = R.TimeElapsed.ToString(),
                        EmergencyType = R.EmergencyTypes.Name,
                        CloseCode = R.CloseCode


                    }).AsQueryable();
        }

        public override Task<Models.Emergency> GetByIdAsync(int id)
        {
            return _context.Emergencies.Include(E=>E.Customer).ThenInclude(E=>E.ApplicationUser)
                .Include(E=>E.ApplicationUser).FirstAsync(E=>E.id==id);
        }
        public async Task<(bool Succeeded, string[] Errors)> CloseRequest(CloseDto close)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
               Models.Emergency Request = await GetByIdAsync(close.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                if (Request.CloseCode != close.Code)
                {
                    return (false, new string[] { "Invaild Code" });

                }
                var result = await _RequestManagmentService.CloseMehtod1(close, (int)ServiceTypeEnum.emergencyRequest);

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

                string url = $"/clientPage/Evaluation/Service/{Request.id}/{Request.RequestCode}/{(int)ServiceTypeEnum.emergencyRequest}/{Request.TechnicianId}";

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

                var Message = MessageSender2.Messages.Close(Request.Customer.ApplicationUser.FullName, short1.Id, Request.RequestCode.ToString());

                var SendMessageResult1 = await _messageSender.Send3Async(Request.Customer.ApplicationUser.PhoneNumber, Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                var SendMessageResult = await _emailService.SendEmailAsync(Request.Customer.ApplicationUser.Email, Message,"اغلاق طلب");
                if (!SendMessageResult.Succeeded)
                {
 
                    return (SendMessageResult.Succeeded, new string[] { SendMessageResult.MessageError });

                }
                _RequestManagmentService.Commit(trans);
                 return (true, null);


            }
            catch (Exception ex)
            {
                 _RequestManagmentService.Rollback(trans);

                return (false, new string[] { "Faild To Close the Request" });
            }


        }

        public async Task<(bool Succeeded, string[] Errors)> TransferRequest(TransferDto transfer)
        {
            //get Request
           Models.Emergency Request = await GetByIdAsync(transfer.RequestId);
            if (Request == null)
            {
                return (false, new string[] { "Request Not Found" });

            }
            var result = await _RequestManagmentService.Transfer(transfer, (int)ServiceTypeEnum.emergencyRequest);

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

        public async Task<(bool Succeeded, string[] Errors)> PostponeRequest(PostponeDto Postpone)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();

            try
            {
                Models.Emergency Request = await GetByIdAsync(Postpone.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                //if (Postpone.file != null)
                //{
                //    var resultt = await _fileService.SaveImage("apertmantServices", Postpone.file);
                //    Postpone.filePath = resultt.imagePath;

                //}

                var result = await _RequestManagmentService.Postpone(Postpone, (int)ServiceTypeEnum.emergencyRequest);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }

                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                Request.RequestStuatus = "تأجيل";
                var result2 = Update(Request);
                if (!result2.Succeeded)
                {
                    return (result2.Succeeded, result2.Errors);


                }
                string Email = Request.Customer.ApplicationUser.Email;



                var SendMessageResult = await _messageSender.Send3Async(Request.Customer.ApplicationUser.PhoneNumber, Postpone.Note, null);
                if (!SendMessageResult)
                {
                    return (false, new string[] { "Faild To send Mail" });

                }

                var resultEmail = await _emailService.SendEmailAsync(Email, Postpone.Note, "Request");
                if (!resultEmail.Succeeded)
                {
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

        public async Task<(bool Succeeded, string[] Errors)> ReplyRequest(ReplyDto replyDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                Models.Emergency Request = await GetByIdAsync(replyDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }

                var result = await _RequestManagmentService.Reply(replyDto, (int)ServiceTypeEnum.emergencyRequest);

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

                var SendMessageResult1 = await _messageSender.Send3Async(Request.Customer.ApplicationUser.PhoneNumber, replyDto.Message, null);
                if (!SendMessageResult1)
                {
                    return (false, null);


                }
                //var SendMessageResult2 = await _emailService.SendEmailAsync(Request.Customer.ApplicationUser.Email, replyDto.Message,"");

                _RequestManagmentService.Commit(trans);

                return (true, null);

            }
            catch (Exception ex)
            {
                _RequestManagmentService.Rollback(trans);

                return (false, new string[] { ex.Message });
            }
        }

        public async Task<(bool Succeeded, string[] Errors)> StartRequest(StartDto startDto)
        {
            var trans = await _RequestManagmentService.BeginTransactionAsync();
            try
            {
                Models.Emergency Request = await GetByIdAsync(startDto.RequestId);
                if (Request == null)
                {
                    return (false, new string[] { "Request Not Found" });

                }
                string Message = $"تم بدء الطلب";


                var result = await _RequestManagmentService.Start(startDto, (int)ServiceTypeEnum.emergencyRequest);

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
                var messageToClient = MessageSender2.Messages.StartRequest(Request.Customer.ApplicationUser.FullName, Request.ApplicationUser.FullName, Request.ApplicationUser.PhoneNumber, Request.RequestCode);


                var SendMessageResult = await _messageSender.Send3Async(Request.Customer.ApplicationUser.PhoneNumber, messageToClient, null);
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

        public IQueryable<EmergencyRequestDto.EmergencyRequestToShow> GetRequestToProjectsManager(List<int> projects,
            string projectName = null, int type = 0, string techniciId = null, string ClientName = null, 
            string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day = 0, int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default)
        {
            var requests = _context.Emergencies.AsNoTracking()
                            .Where(R => projects.Any(userProject => userProject == R.projectId))
                            .AsQueryable();

            if (projectName != null)
            {
                requests = requests.Where(R => R.Project.ProjectName.Contains(projectName));
            }
              if (type != 0)
            {
                requests = requests.Where(R => R.EmergencyId == type);

            }
              if (techniciId != null)
            {
                requests = requests.Where(R => R.TechnicianId == techniciId);

            }
              if (ClientName != null)
            {
                requests = requests.Where(R => R.Customer.ApplicationUser.FullName.Contains(ClientName));

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
                requests = requests.Where(R => R.Customer.ApplicationUser.PhoneNumber.Contains(ClientPhone));

            }
            //else if (day > 0)
            //{
            //    DateOnly currentData = DateOnly.FromDateTime(DateTime.Now);
            //    requests = requests.Where(R => R.RequestStuatus == "جديد" && DateOnly.FromDateTime(R.DateOfVisit) == currentData);


            //}
            //else if (week > 0)
            //{
            //    // تحديد بداية ونهاية الأسبوع الحالي
            //    DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
            //    DateOnly endOfWeek = startOfWeek.AddDays(7);

            //    // فلترة الطلبات خلال هذا الأسبوع
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= startOfWeek && DateOnly.FromDateTime(R.DateOfVisit) < endOfWeek);

            //}
            //else if (month > 0)
            //{
            //    int CurrentMonth = DateTime.Now.Month;
            //    requests = requests.Where(R => R.DateOfVisit.Month == CurrentMonth);

            //}
            //else if (year > 0)
            //{
            //    int CurrentYear = DateTime.Now.Year;

            //    requests = requests.Where(R => R.DateOfVisit.Year == CurrentYear);

            //}
            //if (from != default)
            //{
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) >= from);

            //}
            //if (to != default)
            //{
            //    requests = requests.Where(R => DateOnly.FromDateTime(R.DateOfVisit) <= to && DateOnly.FromDateTime(R.DateOfVisit) >= from);


            //}
            return requests
                    .Include(R => R.Project)
                    .Include(R => R.Customer)
                    .ThenInclude(R => R.ApplicationUser)
                    .Include(R => R.ApplicationUser)
                    .Include(R => R.EmergencyTypes)
                     .OrderByDescending(R => R.id)
                    .Select(R => new EmergencyRequestToShow()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        ClientName = R.Customer.ApplicationUser.FullName,
                        ClientPhone = R.Customer.ApplicationUser.PhoneNumber,
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Customer.City,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        TimeElapsed = R.TimeElapsed.ToString(),
                        EmergencyType = R.EmergencyTypes.Name,
                        CloseCode=R.CloseCode

                    }).AsQueryable();

        }
    

        public EmergencyRequestDto.EmergencyRequestToShow GetRequest(int RequestId)
        {
            List<string> images = _requestImage.GetImages(RequestId, (int)ServiceTypeEnum.emergencyRequest);
            EmergencyRequestToShow emergency =_context.Emergencies
                    .Include(R => R.Project)
                    .Include(R => R.EmergencyTypes)
                     .Include(R => R.Customer)
                    .ThenInclude(R => R.ApplicationUser)
                    .Include(R => R.ApplicationUser)
                     .OrderByDescending(R => R.id)
                    .Select(R => new EmergencyRequestToShow()
                    {
                        id = R.id,
                        RequestCode = R.RequestCode,
                        projectName = R.Project.ProjectName,
                        ClientName = R.Customer.ApplicationUser.FullName,
                        ClientPhone = R.Customer.ApplicationUser.PhoneNumber,
                        CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | HH:mm:ss tt"),
                        Address = R.Customer.City,
                        TechnicianName = R.ApplicationUser.FullName,
                        TechnicianPhone = R.ApplicationUser.PhoneNumber,
                        RequestStuatus = R.RequestStuatus,
                        Description = R.Description,
                        UnitNo = R.Customer.UnitNo.ToString(),
                        BulidingName = R.Customer.BulidingName,
                        TimeElapsed = R.TimeElapsed.ToString(),
                        EmergencyType = R.EmergencyTypes.Name,
                        images = images,
                        CloseCode = R.CloseCode

                    }).FirstOrDefault(R=>R.id == RequestId);
            return emergency;
        }





        public async Task<(bool Succeeded, string[] Errors)> Delete(int id)
        {
            try
            {
               Models.Emergency Emergency = await GetByIdAsync(id);
                if (Emergency != null)
                {
                    _context.Emergencies.Remove(Emergency);
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
