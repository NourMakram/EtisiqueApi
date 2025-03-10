using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenServiceController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly IKitchenService _KitchenService;
        private ICustomerService _customerService;
        private IAcountService _acountService;
        private IRequestManagement _requestManagement;
        private IRequestImage _requestImage;
        public KitchenServiceController(
          IFileService fileService, IKitchenService KitchenService,IAcountService acountService,
           ILocationService locationService, IEmailService emailService, ICustomerService customerService
            ,IRequestManagement requestManagement, IRequestImage requestImage
           )
        {

            _locationService = locationService;
            _fileService = fileService;
            _emailService = emailService;
            _KitchenService = KitchenService;
            _acountService = acountService;
            _requestManagement = requestManagement;
            _customerService = customerService;
            _requestImage = requestImage;

        }
        [HttpPost]
        [Authorize(Policy = "KitchenServices.Add")]
        public async Task<IActionResult> Add(KitchenServiceDto kitchenService)
        {
            if (ModelState.IsValid)
            {
                var trans = await _KitchenService.BeginTransactionAsync();

                try
                {
                    //if (kitchenService.FileUrl != null)
                    //{
                    //    var resultt = await _fileService.SaveImage("KitchenService", kitchenService.FileUrl);
                    //    kitchenService.RequestImage = resultt.imagePath;

                    //}
                    KitchenServices lastOrder = _KitchenService.LastOrder();
                    int RequestCode = 1100;

                    if (lastOrder != null)
                    {
                        RequestCode = (lastOrder.RequestCode + 1);
                    }
                    var customerId = kitchenService.CustomerId;

                    if (kitchenService.CustomerId == 0)
                    {
                        RegisterDto newUser = new RegisterDto()
                        {
                            FullName = kitchenService.ClientName,
                            Email = kitchenService.Email,
                            PhoneNumber = kitchenService.ClientPhone,
                            projectId = kitchenService.projectId,
                            UnitNo = kitchenService.UnitNo,
                            BulidingName = kitchenService.BuildingName,
                            City = kitchenService.Address,
                            Latitude = kitchenService.Latitude,
                            Longitude = kitchenService.Longitude,
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

                    KitchenServices Request = new KitchenServices()
                    {
                        ClientName = kitchenService.ClientName,
                        ClientPhone = kitchenService.ClientPhone,
                        Address = kitchenService.Address,
                        CreatedDate = dateAfter3Hours,
                        Description = kitchenService.Description,
                        CustomerId = customerdb.Id,
                        Period = kitchenService.Period,
                        RequestCode = RequestCode,
                        RequestLocationId = customerdb.Locations.FirstOrDefault()?.Id,
                          
                        RequestStuatus = "جديد",
                        projectId = kitchenService.projectId,
                        BuildingName = kitchenService.BuildingName,
                        UnitNo = kitchenService.UnitNo,
                        Area = kitchenService.Area
                    };
                    var result2 = await _KitchenService.AddAsync(Request);

                    if (!result2.Succeeded)
                    {
                        return BadRequest(result2.Errors);
                    }

                    if (kitchenService.formFiles != null && kitchenService.formFiles.Count() > 0)
                    {
                        var resultt = await _requestImage.Add(kitchenService.formFiles, (int)ServiceTypeEnum.KitchenService, Request.id, "KitchenService");
                        if (!resultt.Succeeded)
                        {
                            return BadRequest(resultt.Errors);
                        }

                    }

                    _KitchenService.Commit(trans);

                    return Ok();

                }
                catch (Exception ex)
                {
                    _KitchenService.Rollback(trans);
                    return BadRequest(ex.Message);

                }
            }
            return BadRequest(ModelState);
        }
        [HttpGet("Filter/{pageNumber}/{pageSize}")]
        [Authorize(Policy = "KitchenServices.view")]
        public  async Task<IActionResult> FilterRequests(string userId,int pageNumber = 1, int pageSize = 7, string projectName = null, string techniciId = null, string ClientName = null,
        string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts=null, int day =0)
        {
            List<int> projects = _acountService.GetUserProjects(userId);
            if(projects.Count() > 0)
            {
                var Requests1 = await _KitchenService.GetRequestToProjectsManager(projects,projectName, techniciId, ClientName, BuildingName, UnitNo, ClientPhone, Code, Stauts, day)
                                 .Select(R => new KitchenServiceResponse()
                                 {
                                     id = R.id,
                                     projectName = R.Project.ProjectName,
                                     BuildingName = R.Customer.BulidingName,
                                     UnitNo = R.Customer.UnitNo,
                                     ClientPhone = R.ClientPhone,
                                     ClientName = R.ClientName,
                                     TechnicianName = R.Technician.FullName,
                                     RequestCode = R.RequestCode,
                                     CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                     RequestStuatus = R.RequestStuatus,
                                      TechnicianPhone = R.Technician.PhoneNumber,
                                     Longitude = R.Location.Longitude,
                                     Latitude = R.Location.Latitude,
                                     AgreementFile = R.AgreementFile,
                                     Area = R.Area,
                                      Description = R.Description,
                                     Period = R.Period,


                                 }).ToPaginatedListAsync(pageNumber, pageSize);
                return Ok(Requests1);
            }
            var Requests =  await _KitchenService.FilterRequestsBy(projectName, techniciId, ClientName, BuildingName, UnitNo, ClientPhone, Code, Stauts, day)
                 .Select(R => new KitchenServiceResponse()
                 {
                     id = R.id,
                     projectName = R.Project.ProjectName,
                     BuildingName = R.BuildingName,
                     UnitNo = R.UnitNo,
                     ClientPhone = R.ClientPhone,
                     ClientName = R.ClientName,
                     TechnicianName = R.Technician.FullName,
                     RequestCode = R.RequestCode,
                     CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                     RequestStuatus = R.RequestStuatus,
                      TechnicianPhone = R.Technician.PhoneNumber,
                     Longitude = R.Location.Longitude,
                     Latitude = R.Location.Latitude,
                     AgreementFile = R.AgreementFile,
                     Area = R.Area,
                      Description = R.Description,
                     Period = R.Period,


                 }).ToPaginatedListAsync(pageNumber, pageSize);
            return Ok(Requests);
        }

        [HttpGet("Get/{id}")]
        [Authorize(Policy = "KitchenServices.Show")]
        public async Task<IActionResult> Get(int id)
        {
            var Request = (await _KitchenService.GetByIdAsync(id));
            if (Request == null)
            {
                return BadRequest();
            }

            var requestDetials = new KitchenServiceResponse()
            {
                id = Request.id,
                projectName = Request.Project.ProjectName,
                BuildingName = Request.BuildingName,
                UnitNo = Request.UnitNo,
                ClientPhone = Request.ClientPhone,
                ClientName = Request.ClientName,
                TechnicianName = Request.Technician?.FullName,
                RequestCode = Request.RequestCode,
                CreatedDate = Request.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                RequestStuatus = Request.RequestStuatus,
                 TechnicianPhone = Request.Technician?.PhoneNumber,
                Longitude = Request.Location.Longitude,
                Latitude = Request.Location.Latitude,
                AgreementFile = Request.AgreementFile,
                Area = Request.Area,
                TimeElapsed= Request.TimeElapsed,
                 Description = Request.Description,
                Period = Request.Period,
                closeCode=Request.CloseCode,
                images=_requestImage.GetImages(id,(int)ServiceTypeEnum.KitchenService)


            };
            return Ok(requestDetials);
        }

        [HttpGet("TechnicianRequests/{TechnicianId}/{Page}/{PageSize}")]
        [Authorize(Policy = "KitchenServices.Technician")]
        public async Task<IActionResult> GetTechnicianRequests(string TechnicianId, int Page = 1, int PageSize = 10, string Status = null, int day=0)
        {
            var Requests = await _KitchenService.GetRequestToTechninics(TechnicianId, Status, day)
                           .Select(R => new KitchenServiceResponse()
                           {
                               id = R.id,
                               projectName = R.Project.ProjectName,
                               BuildingName = R.BuildingName,
                               UnitNo = R.UnitNo,
                               ClientPhone = R.ClientPhone,
                               ClientName = R.ClientName,
                               TechnicianName = R.Technician.FullName,
                               RequestCode = R.RequestCode,
                               CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                               RequestStuatus = R.RequestStuatus,
                                TechnicianPhone = R.Technician.PhoneNumber,
                               Longitude = R.Location.Longitude,
                               Latitude = R.Location.Latitude,
                               AgreementFile = R.AgreementFile,
                               Area = R.Area,
                                TimeElapsed=R.TimeElapsed,
                               Description = R.Description,
                               Period = R.Period,


                           }).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);

        }

        [HttpGet("ClientRequests/{CustomerId}/{Page}/{PageSize}")]
        [Authorize(Policy = "KitchenServices.Client")]
        public async Task<IActionResult> GetClientRequests(string CustomerId, int Page = 1, int PageSize = 10, string Status = null, string code = null, DateOnly date = default)
        {
            var Requests = await _KitchenService.GetRequestToCustomer(CustomerId, code, Status, date)
                            .Select(R => new KitchenServiceResponse()
                            {
                                id = R.id,
                                projectName = R.Project.ProjectName,
                                BuildingName = R.BuildingName,
                                UnitNo = R.UnitNo,
                                ClientPhone = R.ClientPhone,
                                ClientName = R.ClientName,
                                TechnicianName = R.Technician.FullName,
                                RequestCode = R.RequestCode,
                                CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                                RequestStuatus = R.RequestStuatus,
                                 TechnicianPhone = R.Technician.PhoneNumber,
                                Longitude = R.Location.Longitude,
                                Latitude = R.Location.Latitude,
                                AgreementFile = R.AgreementFile,
                                Area = R.Area,
                                Description = R.Description,
                                Period = R.Period,


                            }).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);
        }
        [HttpPost("Transfer")]
        [Authorize(Policy = "KitchenServices.transfer")]
        public async Task<IActionResult> TransferRequest(TransferDto transfer)
        {
            var result = await _KitchenService.Transfer(transfer);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Close/Code")]
        [Authorize(Policy = "KitchenServices.CloseM1")]
        public async Task<IActionResult> CloseByCodeRequest(CloseDto close)
        {
            var result = await _KitchenService.CloseByCode(close);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();


        }
        [HttpPost("Close/Note")]
        [Authorize(Policy = "KitchenServices.CloseM1")]
        public async Task<IActionResult> CloseByNoteRequest(CloseByNoteDto close)
        {
            var result = await _KitchenService.CloseByNote(close);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();


        }

        [HttpPost("Reply")]
        [Authorize(Policy = "KitchenServices.Repley")]
        public async Task<IActionResult> Reply(ReplyDto reply)
        {
            if (ModelState.IsValid)
            {
                var Result = await _KitchenService.Reply(reply);

                if (!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }
                return Ok();
            }
            return BadRequest("Faild To Reply");

        }
        [HttpPost("Agreemant")]
        [Authorize(Policy = "KitchenServices.Agreemant")]
        public async Task<IActionResult> uploadAgreemant(AgreemantFileDto Agreemant)
        {
   
            if (Agreemant.file != null)
            {
                var resultt = await _fileService.SaveImage("KitchenService", Agreemant.file);
                if (!resultt.Succeeded)
                {
                    return BadRequest("Faild To Upload File");

                }
                Agreemant.filePath = resultt.imagePath;
            }
            var result = await _KitchenService.UploadAgreement(Agreemant);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();

        }

        [HttpPost("Start")]
        [Authorize(Policy = "KitchenServices.Start")]
        public async Task<IActionResult> Start(StartDto start)
        {
            var Result = await _KitchenService.Start(start);

            if (!Result.Succeeded)
            {
                return BadRequest(Result.Errors);
            }
            return Ok();

        }

        [HttpGet("Track/{requestId}")]
        [Authorize(Policy = "KitchenServices.Track")]
        public IActionResult RequestTrack(int requestId)
        {
            return Ok(
           _requestManagement.GetRequestManagements(requestId, (int)ServiceTypeEnum.KitchenService)
           .Select(M => new
           {
               Id = M.Id,
               ProduceType = M.ProcedureType,
               date = M.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
               Attachments = M.Attachments.Select(n => n.imagePath).ToList(),
               Notes = M.Notes,
               CreateBy = M.CreatedBy.FullName

           }).ToList()
            );
        }
        [HttpDelete]
        [Authorize(policy: "KitchenServices.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _KitchenService.Delete(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        //   [HttpGet("ProjectsManager/{UserId}/{pageNumber}/{pageSize}")]
        //   [Authorize(policy: "KitchenServices.projectManager")]
        //   public async Task<IActionResult> GetProjectsToManager(string UserId,int pageNumber = 1, int pageSize = 7, string projectName = null, string techniciId = null, string ClientName = null,
        //   string BuildingName = null, int UnitNo = 0, string ClientPhone = null, string Code = null, string Stauts = null, int day = 0)
        //   {
        //       var Requests = await _KitchenService.GetRequestToProjectsManager(UserId, projectName, techniciId, ClientName, BuildingName, UnitNo, ClientPhone, Code, Stauts, day)
        //.Select(R => new KitchenServiceResponse()
        //{
        //    id = R.id,
        //    projectName = R.Project.ProjectName,
        //    BuildingName = R.BuildingName,
        //    UnitNo = R.UnitNo,
        //    ClientPhone = R.ClientPhone,
        //    ClientName = R.ClientName,
        //    TechnicianName = R.Technician.FullName,
        //    RequestCode = R.RequestCode,
        //    CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
        //    RequestStuatus = R.RequestStuatus,
        //    NumDays = R.NumDays,
        //    TechnicianPhone = R.Technician.PhoneNumber,
        //    Longitude = R.Location.Longitude,
        //    Latitude = R.Location.Latitude,
        //    AgreementFile = R.AgreementFile,
        //    Area = R.Area,
        //    RequestImage = R.RequestImage,
        //    Description = R.Description,
        //    Period = R.Period,


        //}).ToPaginatedListAsync(pageNumber, pageSize);
        //       return Ok(Requests);

        //   }

    }


}
