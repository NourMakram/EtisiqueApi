using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using static EcommercePro.Models.Constants;
using static EtisiqueApi.DTO.EmergencyRequestDto;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentServicesRequestController : ControllerBase
    {
        private readonly IApartmentServicesRequestService _apartmentServices;
        private readonly ICustomerService _customerService;
        private readonly ILocationService _locationService;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly IAcountService _acountService;
        private readonly IRequestManagement _requestManagement;
        private readonly ISubServiceRequestService _subServiceRequestService;
        private readonly MessageSender2.IMessageSender _messageSender;
        private IRequestImage _requestImage;
        public ApartmentServicesRequestController(IApartmentServicesRequestService apartmentServices,
            ICustomerService customerService, IFileService fileService, IRequestManagement requestManagement,
            ILocationService locationService, IEmailService emailService, IAcountService acountService,
             ISubServiceRequestService subServiceRequestService, IRequestImage requestImage, MessageSender2.IMessageSender messageSender)
        {
            _apartmentServices = apartmentServices;
            _customerService = customerService;
            _locationService = locationService;
            _fileService = fileService;
            _emailService = emailService;
            _requestImage = requestImage;
            _acountService = acountService;
            _requestManagement = requestManagement;
             _subServiceRequestService = subServiceRequestService;
            _messageSender = messageSender;
        }
        [HttpPost]
       [Authorize(policy: "maintenanceRequests.Add")]
         public async Task<IActionResult> AddService(ApartmentServicesDto apartmentService)
        {


            if (ModelState.IsValid)
            {
                var trans = await _apartmentServices.BeginTransactionAsync();

                try
                {
                     
                    ApartmentServicesRequest lastOrder = _apartmentServices.LastOrder(apartmentService.hasGuarantee);
                    int RequestCode = 1100;

                    if (lastOrder != null)
                    {
                         RequestCode = (lastOrder.RequestCode + 1);

                    }
                     
                    //genetate Code
                    Random generator = new Random();
                    string CloseCode = generator.Next(0, 10000).ToString("D4");
                    var customerId = apartmentService.CustomerId;
                    if(apartmentService.CustomerId == 0)
                    {
                        RegisterDto newUser = new RegisterDto()
                        {
                            FullName = apartmentService.ClientName,
                            Email = apartmentService.Email,
                            PhoneNumber = apartmentService.ClientPhone,
                            projectId = apartmentService.projectId,
                            UnitNo = apartmentService.UnitNo,
                            BulidingName = apartmentService.BuildingName,
                            City = apartmentService.Address,
                            Latitude = apartmentService.Latitude,
                            Longitude = apartmentService.Longitude,
                            Password = "Etsaq@2000",
                            TypeProject = "مالك"
                        };
                        var ResultData = await _acountService.AddNewClient(newUser);

                        if (!ResultData.Succeeded)
                        {
                            return BadRequest(ResultData.Errors);
                        }
                        customerId = ResultData.id;
                    }
                    Customer customerdb = await _customerService.GetByIdAsync(customerId);
                    if (customerdb == null)
                    {
                        return BadRequest("Customer Not Found");
                    }
                    var currentDate = DateTime.UtcNow;

                    // Increase the hour by 3
                    var dateAfter3Hours = currentDate.AddHours(3);

                    ApartmentServicesRequest Request = new ApartmentServicesRequest()
                    {
                        ServiceTypeId = apartmentService.ServiceTypeId,
                        ClientName = apartmentService.ClientName,
                        ClientPhone = apartmentService.ClientPhone,
                        Address = apartmentService.Address,
                        DateOfVisit = apartmentService.DateOfVisit,
                        CreatedDate = dateAfter3Hours,
                        Description = apartmentService.Description,
                        CustomerId = customerdb.Id,
                        Period = apartmentService.Period,
                        RequestCode = RequestCode,
                        RequestLocationId = customerdb.Locations.FirstOrDefault()?.Id,
                         hasGuarantee=apartmentService.hasGuarantee,
                         CloseCode = CloseCode,
                        RequestStuatus = "جديد",
                        projectId =apartmentService.projectId,
                        BuildingName=apartmentService.BuildingName,
                        UnitNo=apartmentService.UnitNo,
                    };
                    var result2 = await _apartmentServices.AddAsync(Request);

                    if (!result2.Succeeded)
                    {
                        return BadRequest(result2.Errors);
                    }

                    if (apartmentService.formFiles != null && apartmentService.formFiles.Count() > 0)
                    {
                        var resultt = await _requestImage.Add(apartmentService.formFiles, (int)ServiceTypeEnum.ApartmentService, Request.id, "apertmantServices");
                        if (!resultt.Succeeded)
                        {
                            return BadRequest(resultt.Errors);
                        }

                    }

                    if (apartmentService.SubServices!=null && apartmentService.SubServices.Count > 0)
                    {
                        var result3 =  _subServiceRequestService.AddSubServices(Request.id, apartmentService.SubServices ,(int)ServiceTypeEnum.ApartmentService);
                        if (!result3.Succeeded)
                        {
                            return BadRequest(result3.Errors);
                        }
                    }

                    var Message = MessageSender2.Messages.NewRequestMsg(Request.ClientName,Request.RequestCode,Request.CloseCode);

                    var SendMessageResult = await _messageSender.Send3Async(Request.ClientPhone, Message, null);
                    if (!SendMessageResult)
                    {
                        return BadRequest("Faild To send Message");

                    }
                    var results = await _emailService.SendEmailAsync(apartmentService.Email, Message, "اتساق");
                    if (!results.Succeeded)
                    {
                        return BadRequest(results.MessageError);

                    }
                    _apartmentServices.Commit(trans);

                    return Ok();

                }
                catch (Exception ex)
                {
                    _apartmentServices.Rollback(trans);
                    return BadRequest(ex.Message);

                }
            }
            return BadRequest(ModelState);

        }

        [HttpGet("ApartmentServices")]
        [AllowAnonymous]
        public IActionResult ApartmentServices()
        {
            var Services = _subServiceRequestService.GetServices().Select(Service => new { Service.Id, Service.Name });

            return Ok(Services);
        }

        [HttpGet("ApartmentServicesType/{serviceId}")]
        [AllowAnonymous]
        public IActionResult ApartmentServices(int serviceId)
        {
            var ServicesTypes = _subServiceRequestService.GetApartmentServiceTypeByServiceId(serviceId)
                .Select(Service => new { Service.Id, Service.Name });

            return Ok(ServicesTypes);
        }
        [HttpGet("GetRequest/{id}")]
        [Authorize(policy: "maintenanceRequests.Show")]
        public IActionResult GetRequest(int id)
        {
            List<ApartmentRequestDto> Request = _apartmentServices.GetRequest(id);
            return Ok(Request);
        }
        [HttpGet("filter/{Page}/{PageSize}")]
        [Authorize(policy:"maintenanceRequests.view")]
        public async Task<IActionResult> FitlertGetRequests(string UserId, bool hasGuarantee, int Page = 1, int PageSize = 10, string projectName = null,
            int TypeServiceId = 0,string techniciId = null, string ClientName = null,int ConfRequest=0,
            string BuldingName = null, int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day=0
            ,int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default)
        
         {
            List<int> Projects = _acountService.GetUserProjects(UserId);
            if(Projects.Count() > 0)
            {
                var Requests1 = await _apartmentServices.GetRequestToProjectsManager(UserId,Projects, hasGuarantee, projectName, TypeServiceId, techniciId,
               ClientName, BuldingName, UnitNo, ClientPhone, Status, Code, day,week,year,month,from,to,ConfRequest)
              .ToPaginatedListAsync(Page, PageSize);
                return Ok(Requests1);
            }

            var Requests = await _apartmentServices.FilterRequestsBy(hasGuarantee, projectName, TypeServiceId,techniciId,
                ClientName,BuldingName,UnitNo,ClientPhone,Status,Code,day, week, year, month, from, to, ConfRequest)
               .ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);
        }

        [HttpGet("ClientRquests/{CustomerId}/{Page}/{PageSize}")]
        [Authorize(policy: "maintenanceRequests.Clients")]
         public async Task<IActionResult> GetClientRquests(int CustomerId, bool hasGuarantee, int Page = 1, int PageSize = 10 ,string Status=null , string code = null, DateOnly date = default)
        {
            var Requests = await _apartmentServices.GetRequestToCustomer(CustomerId,hasGuarantee,code,Status ,date).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);
        }
       
        [HttpGet("TechnicianRquests/{TechnicianId}/{Page}/{PageSize}")]
        [Authorize(policy: "maintenanceRequests.Technicians")]
         public async Task<IActionResult> GetTechnicianRquests(string TechnicianId, bool hasGuarantee,int Page = 1, int PageSize = 10 , string Status = null, int day=0)
        {
            var Requests = await _apartmentServices.GetRequestToTechninics(TechnicianId,  hasGuarantee, Status, day).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);

        }

        [HttpPost("Transfer")]
        [Authorize(policy: "maintenanceRequests.transfer")]
        public async Task<IActionResult> TransferRequest(TransferDto transfer)
        {
            var result = await _apartmentServices.TransferRequest(transfer);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Close")]
        [Authorize(policy: "maintenanceRequests.close")]
        public async Task<IActionResult> CloseRequest(CloseDto close)
        {
            
            var result = await _apartmentServices.CloseRequest(close);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();


        }
        [HttpPost("Postpone")]
        [Authorize(policy: "maintenanceRequests.Delay")]
        public async Task<IActionResult> PostponeRequest(PostponeDto postpone)
        {
            
            var result = await _apartmentServices.PostponeRequest(postpone);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            ApartmentServicesRequest Request = await _apartmentServices.GetByIdAsync(postpone.RequestId);
            string Email = Request.Customer.ApplicationUser.Email;



            var SendMessageResult = await _messageSender.Send3Async(Request.ClientPhone, postpone.Note, null);
            if (!SendMessageResult)
            {
                return BadRequest("Faild To send Message");

            }

            var resultEmail = await _emailService.SendEmailAsync(Email, postpone.Note, "Request");
            if (!resultEmail.Succeeded)
            {
                return BadRequest("Faild To Send Message ");
            }
            return Ok();

        }

        [HttpPost("Reply")]
        [Authorize(policy: "maintenanceRequests.Reply")]
        public async Task<IActionResult> Reply(ReplyDto reply)
        {
            if (ModelState.IsValid)
            {
                var Result = await _apartmentServices.ReplyRequest(reply);

                if (!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }
                return Ok();
            }
            return BadRequest("Faild To Reply");

        }

        [HttpPost("Start")]
        [Authorize(policy: "maintenanceRequests.Start")]
        public async Task<IActionResult> Start(StartDto start)
        {
            var Result = await _apartmentServices.StartRequest(start);

            if (!Result.Succeeded)
            {
                return BadRequest(Result.Errors);
            }
            return Ok();

        }

        [HttpGet("Track/{requestId}/{ServiceType}")]
        [Authorize(policy: "maintenanceRequests.Track")]
        public IActionResult RequestTrack(int requestId ,int ServiceType)
        {
           return Ok(
               
          _requestManagement.GetRequestManagements(requestId,ServiceType)
          .Select(M=>new
           {
               Id = M.Id,
               ProduceType=M.ProcedureType,
               date = M.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
               Attachments = M.Attachments.Select(n=>n.imagePath).ToList(),
               Notes=M.Notes,
               CreateBy =M.CreatedBy.FullName 

           }).ToList()
           );
        }

        [HttpGet("Change")]
        [AllowAnonymous]
        public IActionResult change()
        {
            var result = _apartmentServices.change();
            if(result== true)
            {
                return Ok();
            }
            return BadRequest();
        }

       [HttpPost("Approve")]
        [Authorize(policy: "maintenanceRequests.Transfer2")]
        public async Task <IActionResult> Approve(ApproveRequestDto ApproveRequestDto)
        {
           var result = await _apartmentServices.ApproveRequestMethod2(ApproveRequestDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Accept")]
        [Authorize(policy: "maintenanceRequests.Accept")]
        public async Task<IActionResult> Accept(ReplyDto AcceptRequestDto)
        {
            var result = await _apartmentServices.AcceptRequest(AcceptRequestDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Refuse")]
        [Authorize(policy: "maintenanceRequests.Refuse")]
        public async Task<IActionResult> Refuse(ReplyDto RefuseRequestDto)
        {
            var result = await _apartmentServices.RefuseRequest(RefuseRequestDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }


    }
}
