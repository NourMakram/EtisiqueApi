using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Globalization;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonPartsController : ControllerBase
    {
        ICommonPartsService _CommonPartsService;
        IFileService _fileService;
        ISubServiceRequestService _subServiceRequestService;
        IRequestManagement _requestManagement;
        IAcountService _acountService;
        private IRequestImage _requestImage;
        public CommonPartsController(ICommonPartsService CommonPartsService, IFileService fileService,
            ISubServiceRequestService subServiceRequestService, IRequestImage requestImage,IRequestManagement requestManagement, IAcountService acountService) {

            _CommonPartsService= CommonPartsService;    
            _fileService = fileService;
            _subServiceRequestService = subServiceRequestService;
            _requestManagement = requestManagement;
            _acountService = acountService;
            _requestImage = requestImage;


        }
        [HttpPost("Add")]
        [Authorize(policy: "commonPartsRequest.Add")]
        public async Task<IActionResult> Add(CommonPartsDto commonParts)
        {
            if (ModelState.IsValid)
            {
   
                try
                {
                    
                    RequestsCommonParts lastOrder = _CommonPartsService.LastOrder(commonParts.IsCleaning);
                    int RequestCode = 1100;

                    if (lastOrder != null)
                    {
                        RequestCode = (lastOrder.RequestCode + 1);

                    }

                    var currentDate = DateTime.UtcNow;

                    // Increase the hour by 3
                    var dateAfter3Hours = currentDate.AddHours(3);

                    commonParts.DateOfVisit = commonParts.DateOfVisit.AddHours(dateAfter3Hours.Hour);
                    commonParts.DateOfVisit = commonParts.DateOfVisit.AddMinutes(dateAfter3Hours.Minute);

                    RequestsCommonParts Request = new RequestsCommonParts()
                    {
                        RequestCode = RequestCode,
                        IsUrgent = commonParts.IsUrgent ,
                        ManagerId = commonParts.ManagerId,
                        BuildingName=commonParts.BuildingName,
                        UnitNo =commonParts.UnitNo,
                        Type = commonParts.Type,
                        CreatedDate=dateAfter3Hours ,
                        RequestStuatus="جديد",
                        DateOfVisit=commonParts.DateOfVisit,
                        Description=commonParts.Description,
                        Position=commonParts.Position,
                        projectId=commonParts.projectId,
                        ServiceTypeId=commonParts.ServiceTypeId,
                        UpdatedDate= dateAfter3Hours,
                        IsCleaning=commonParts.IsCleaning

                    };

                    
                    var result2 = await _CommonPartsService.AddAsync(Request);

                    if (!result2.Succeeded)
                    {
                        return BadRequest(result2.Errors);
                    }

                    if (commonParts.formFiles != null && commonParts.formFiles.Count() > 0)
                    {
                        var resultt = await _requestImage.Add(commonParts.formFiles, (int)ServiceTypeEnum.CommonParts, Request.id, "CommonParts");
                        if (!resultt.Succeeded)
                        {
                            return BadRequest(resultt.Errors);
                        }

                    }

                    if (commonParts.SubServices != null && commonParts.SubServices.Count > 0)
                    {
                        var result3 =   _subServiceRequestService.AddSubServices(Request.id, commonParts.SubServices, (int)ServiceTypeEnum.CommonParts);
                        if (!result3.Succeeded)
                        {
                            return BadRequest(result3.Errors);
                        }
                    }
                    
 
                    return Ok();

                }
                catch (Exception ex)
                {
                     return BadRequest(ex.Message);

                }
            }
            return BadRequest(ModelState);

        }
        [HttpGet("{id}")]
        [Authorize(policy: "commonPartsRequest.Show")]
        public IActionResult Get(int id)
        {
            return Ok(_CommonPartsService.Get(id));
        }
        [HttpGet("{Page}/{PageSize}")]
        [Authorize(policy: "commonPartsRequest.view")]
        public async Task<IActionResult> Requests(string UserId, bool iscleaning,int Page=1, int PageSize=5,string projectName=null,
            int TypeServiceId=0, string techniciId=null,
            string ManagerId=null, string BuildingName=null, string Status=null, string Code=null  , bool IsUrget = false,
            int day=0,
            int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default, int ConfRequest = 0)
        {

            List<int> projects = _acountService.GetUserProjects(UserId);
            if(projects.Count()> 0)
            {
                var requests1 =await _CommonPartsService.GetRequestToProjectsManager(UserId,projects,iscleaning, projectName, TypeServiceId, 
                    techniciId, ManagerId, BuildingName, Status, Code, IsUrget, day,week,year,month,from,to,ConfRequest)
                    .Select(R => new CommonPartsResponse()
                {
                    id = R.id,
                    RequestCode = R.RequestCode,
                    RequestStuatus = R.RequestStuatus,
                    projectName = R.Project.ProjectName,
                    ManagerName = R.Manager.FullName,
                    TechnicianName = R.Technician.FullName,
                    IsUrgent = R.IsUrgent==true?"نعم" : "لا",
                    CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy "),
                    UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    BuildingName = R.BuildingName,
                    Position = R.Position,
                    ServiceType = R.ServiceType.Name,
                    UnitNo = R.UnitNo,
                    Type = R.Type == 1 ? "داخل" : "خارج",
                    ManagerNote=R.ManagerNote,
                    ProjectId = R.projectId,
                    timeElasped = R.TimeElapsed,
                    TimeElapsedClose1=R.TimeElapsedClose1,
                    TimeElapsedClose2=R.TimeElapsedClose2

                    })
                .ToPaginatedListAsync(Page, PageSize);
                return Ok(requests1);
            }

            var Requests = await _CommonPartsService.FilterRequestsBy(iscleaning,projectName,TypeServiceId,techniciId,ManagerId,
                BuildingName,Status,Code,IsUrget, day, week, year, month, from, to,ConfRequest)
                .Select(R => new CommonPartsResponse()
                {
                    id = R.id,
                    RequestCode = R.RequestCode,
                    RequestStuatus = R.RequestStuatus,
                    projectName = R.Project.ProjectName,
                    ManagerName = R.Manager.FullName,
                    TechnicianName = R.Technician.FullName,
                     IsUrgent = R.IsUrgent == true ? "نعم" : "لا",
                    CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy "),
                    UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    BuildingName = R.BuildingName,
                    Position = R.Position,
                    ServiceType = R.ServiceType.Name,
                    UnitNo = R.UnitNo,
                    Type=R.Type==1 ?"داخل" : "خارج",
                    ManagerNote = R.ManagerNote,
                    ProjectId = R.projectId,
                    timeElasped=R.TimeElapsed,
					TimeElapsedClose1 = R.TimeElapsedClose1,
					TimeElapsedClose2 = R.TimeElapsedClose2


				})
                .ToPaginatedListAsync(Page,PageSize);
            return Ok(Requests);

        }

        [HttpPost("Transfer")]
        [Authorize(policy: "commonPartsRequest.transfer")]
        public async Task<IActionResult> Transfer(TransferDto transferDto)
        {
            var result =await _CommonPartsService.Transfer(transferDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Close")]
        [Authorize(policy: "commonPartsRequest.close")]
        public async Task<IActionResult> Close(CommonPartsmanagmentDto closeDto)
        {
           
            var result = await _CommonPartsService.Close(closeDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("PartialClose")]
        [Authorize(policy: "commonPartsRequest.PartialClose")]
        public async Task<IActionResult> PartialClose(CommonPartsmanagmentDto partialCloseDto)
        {
           
            var result = await _CommonPartsService.PartialClose(partialCloseDto,"اقفال جزئى", "اقفال جزئى");
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("PartialClose1")]
        [Authorize(policy: "commonPartsRequest.PartialClose1")]
        public async Task<IActionResult> PartialClose1(CommonPartsmanagmentDto partialCloseDto1)
        {
            try
            {
                
                var result = await _CommonPartsService.PartialClose(partialCloseDto1, "اقفال اول", "اقفال اول");
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);


            }
        }
        [HttpPost("PartialClose2")]
        [Authorize(policy: "commonPartsRequest.PartialClose2")]
        public async Task<IActionResult> PartialClose2(CommonPartsmanagmentDto partialCloseDto2)
        {
           
            var result = await _CommonPartsService.PartialClose(partialCloseDto2, "اقفال ثانى", "اقفال ثانى");
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPost("Refuse")]
        [Authorize(policy: "commonPartsRequest.Refuse")]
        public async Task<IActionResult> Refuse(CloseByNoteDto RefuseData)
        {
           
            var result = await _CommonPartsService.Refuse(RefuseData);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Note")]
        [Authorize(policy: "commonPartsRequest.Note")]
        public async Task<IActionResult> Note(CloseByNoteDto NoteData)
        {
            
            var result = await _CommonPartsService.Note(NoteData);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPost("Confirm")]
        [Authorize(policy: "commonPartsRequest.Confirm")]
        public async Task<IActionResult> Confirm(StartDto ConfirmData)
        {
            
            var result = await _CommonPartsService.Confirm(ConfirmData);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Up")]
        [Authorize(policy: "commonPartsRequest.Up")]
        public async Task<IActionResult> Up(CommonPartsmanagmentDto UpData)
        {
            
            var result = await _CommonPartsService.Up(UpData);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Comment")]
        [Authorize(policy: "commonPartsRequest.Comment")]
        public async Task<IActionResult> Comment(CommonPartsmanagmentDto partialCloseDto)
        {
           
            var result = await _CommonPartsService.Comment(partialCloseDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
       
        [HttpGet("Track/{requestId}")]
        [Authorize(policy: "commonPartsRequest.Track")]
        public IActionResult Track(int requestId, int ServiceType=(int)ServiceTypeEnum.CommonParts)
        {
            return Ok(

           _requestManagement.GetRequestManagements(requestId, (int)ServiceTypeEnum.CommonParts)
           .Select(M => new
           {
               Id = M.Id,
               ProduceType = M.ProcedureType,
               date = M.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
               Attachments = M.Attachments.Select(image=>image.imagePath).ToList(),
               Notes = M.Notes,
               CreateBy = M.CreatedBy.FullName

           }).ToList()
            );
        }

        [HttpGet("Techincan/{techinchId}/{page}/{PageSize}")]
        [Authorize(policy: "commonPartsRequest.Technician")]
        public async Task<IActionResult> GetTechincianRequest(string techinchId, bool iscleaning, int page =1 , int PageSize=10, string Status = null, int day=0)
        {

            var Requests = await _CommonPartsService.GetTechincanRequest(iscleaning,techinchId, Status, day)
                .Select(R => new CommonPartsResponse()
                {
                    id = R.id,
                    RequestCode = R.RequestCode,
                    RequestStuatus = R.RequestStuatus,
                    projectName = R.Project.ProjectName,
                    ManagerName = R.Manager.FullName,
                    TechnicianName = R.Technician.FullName,  
                    Description = R.Description,
                    IsUrgent = R.IsUrgent == true ? "نعم" : "لا",
                    CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                    BuildingName = R.BuildingName,
                    Position = R.Position,
                    ServiceType = R.ServiceType.Name,
                    UnitNo = R.UnitNo,
                    Type = R.Type == 1 ? "داخل" : "خارج",
					TimeElapsedClose1 = R.TimeElapsedClose1,
					TimeElapsedClose2 = R.TimeElapsedClose2,
                    timeElasped=R.TimeElapsed
				})
               .ToPaginatedListAsync(page, PageSize);

            return Ok(Requests);
                

        }
        [HttpGet("Manager/{ManagerId}/{page}/{pageSize}")]
        [Authorize(policy: "commonPartsRequest.Manager")]
        public async Task<IActionResult> GetManagerRequest(string ManagerId, bool iscleaning, int page=1,int pageSize=10, string Status = null, int day=0)
        {

            var Requests = await _CommonPartsService.GetManagerRequest(iscleaning, ManagerId, Status,day)
               .Select(R => new CommonPartsResponse()
               {
                   id = R.id,
                   RequestCode = R.RequestCode,
                   
                   RequestStuatus = R.RequestStuatus,
                   projectName = R.Project.ProjectName,
                   ManagerName = R.Manager.FullName,
                   TechnicianName = R.Technician.FullName,
                   
                   Description = R.Description,
                   IsUrgent = R.IsUrgent == true ? "نعم" : "لا",
                   CreatedDate = R.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                   DateOfVisit = R.DateOfVisit.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                   UpdatedDate = R.UpdatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
                   BuildingName = R.BuildingName,
                   Position = R.Position,
                   ServiceType = R.ServiceType.Name,
                   UnitNo = R.UnitNo,
                   Type = R.Type == 1 ? "داخل" : "خارج",
                   ProjectId = R.projectId,
				   TimeElapsedClose1 = R.TimeElapsedClose1,
				   TimeElapsedClose2 = R.TimeElapsedClose2,
		           timeElasped = R.TimeElapsed

			   })
              .ToPaginatedListAsync(page, pageSize);

            return Ok(Requests);
        }

		[HttpPost("Approve")]
		[Authorize(policy: "commonPartsRequest.transfer2")]
		public async Task<IActionResult> Approve(ApproveRequestDto ApproveRequestDto)
		{
			var result = await _CommonPartsService.ApproveRequestMethod2(ApproveRequestDto);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok();
		}
		[HttpPost("AcceptBaptism")]
		[Authorize(policy: "commonPartsRequest.AcceptBaptism")]
		public async Task<IActionResult> Accept(ReplyDto AcceptRequestDto)
		{
			var result = await _CommonPartsService.AcceptRequest(AcceptRequestDto);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok();
		}
		[HttpPost("RefuseBaptism")]
		[Authorize(policy: "commonPartsRequest.RefuseBaptism")]
		public async Task<IActionResult> Refuse(ReplyDto RefuseRequestDto)
		{
			var result = await _CommonPartsService.RefuseRequest(RefuseRequestDto);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok();
		}

        [HttpDelete]
        [Authorize(policy: "commonPartsRequest.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _CommonPartsService.Delete(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        //[HttpGet("Change")]
        //[AllowAnonymous]
        //public async Task<IActionResult> change()
        //{
        //    var result = await _CommonPartsService.ChangeData();
        //    if (result.Succeeded == true)
        //    {
        //        return Ok();
        //    }
        //    return BadRequest();
        //}



    }


}
