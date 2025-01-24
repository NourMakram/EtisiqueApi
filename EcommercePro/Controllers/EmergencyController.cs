using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyController : ControllerBase
    {
        IEmergencyService _emergencyService;
        IAcountService _acountService;
        IRequestManagement _requestManagement;
       public EmergencyController(IEmergencyService emergencyService, IAcountService acountService, IRequestManagement requestManagement)
        {
            _emergencyService =emergencyService;
            _acountService = acountService;
            _requestManagement = requestManagement;
        }
        [HttpPost]
        [Authorize(policy: "EmergencyRequests.Add")]
        public async Task<IActionResult> Add(EmergencyRequestDto.EmergencyRequestToAdd emergencyRequestToAdd)
        {


            if (ModelState.IsValid)
            {
  
                try
                {
                     var result=await _emergencyService.Add(emergencyRequestToAdd);

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
            return BadRequest(ModelState);

        }

       

         
        [HttpGet("GetRequest/{id}")]
        [Authorize(policy: "EmergencyRequests.Show")]

        public IActionResult GetRequest(int id)
        {
            EmergencyRequestDto.EmergencyRequestToShow Request = _emergencyService.GetRequest(id);
            return Ok(Request);
        }
        [HttpGet("filter/{Page}/{PageSize}")]
        [Authorize(policy: "EmergencyRequests.view")]
        public async Task<IActionResult> FitlertGetRequests(string UserId, int Page = 1, int PageSize = 10, string projectName = null,
            int TypeServiceId = 0, string techniciId = null, string ClientName = null,
            string BuldingName = null, int UnitNo = 0, string ClientPhone = null, string Status = null, string Code = null, int day = 0
            , int week = 0, int year = 0, int month = 0, DateOnly from = default, DateOnly to = default)

        {
            List<int> Projects = _acountService.GetUserProjects(UserId);
            if (Projects.Count() > 0)
            {
                var Requests1 = await _emergencyService.GetRequestToProjectsManager(Projects, projectName, TypeServiceId, techniciId,
               ClientName, BuldingName, UnitNo, ClientPhone, Status, Code, day, week, year, month, from, to)
                    .ToPaginatedListAsync(Page, PageSize);
                return Ok(Requests1);
            }

            var Requests = await _emergencyService.FilterRequestsBy(projectName, TypeServiceId, techniciId,
                ClientName, BuldingName, UnitNo, ClientPhone, Status, Code, day, week, year, month, from, to)
               .ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);
        }

        [HttpGet("ClientRquests/{CustomerId}/{Page}/{PageSize}")]
        [Authorize(policy: "EmergencyRequests.Clients")]

        public async Task<IActionResult> GetClientRquests(int CustomerId, int Page = 1, int PageSize = 10, string Status = null, string code = null, DateOnly date = default)
        {
            var Requests = await _emergencyService.GetRequestToCustomer(CustomerId, code, Status, date).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);
        }

        [HttpGet("TechnicianRquests/{TechnicianId}/{Page}/{PageSize}")]
        [Authorize(policy: "EmergencyRequests.Technicians")]

        public async Task<IActionResult> GetTechnicianRquests(string TechnicianId, int Page = 1, int PageSize = 10, string Status = null, int day = 0)
        {
            var Requests = await _emergencyService.GetRequestToTechninics(TechnicianId, Status, day).ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);

        }

        [HttpPost("Transfer")]
        [Authorize(policy: "EmergencyRequests.transfer")]
        public async Task<IActionResult> TransferRequest(TransferDto transfer)
        {
            var result = await _emergencyService.TransferRequest(transfer);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Close")]
        [Authorize(policy: "EmergencyRequests.close")]
        public async Task<IActionResult> CloseRequest(CloseDto close)
        {
            //if (close.file != null)
            //{
            //    var resultt = await _fileService.SaveImage("apertmantServices", close.file);
            //    close.filePath = resultt.imagePath;

            //}
            var result = await _emergencyService.CloseRequest(close);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();


        }
        [HttpPost("Postpone")]
        [Authorize(policy: "EmergencyRequests.Add")]
        public async Task<IActionResult> PostponeRequest(PostponeDto postpone)
        {
            
            var result = await _emergencyService.PostponeRequest(postpone);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
             
            return Ok();

        }

        [HttpPost("Reply")]
        [Authorize(policy: "EmergencyRequests.Reply")]

        public async Task<IActionResult> Reply(ReplyDto reply)
        {
            if (ModelState.IsValid)
            {
                var Result = await _emergencyService.ReplyRequest(reply);

                if (!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }
                return Ok();
            }
            return BadRequest("Faild To Reply");
        }

        [HttpPost("Start")]
        [Authorize(policy: "EmergencyRequests.Start")]
        public async Task<IActionResult> Start(StartDto start)
        {
            var Result = await _emergencyService.StartRequest(start);

            if (!Result.Succeeded)
            {
                return BadRequest(Result.Errors);
            }
            return Ok();

        }

        [HttpGet("Track/{requestId}/{ServiceType}")]
        [Authorize(policy: "EmergencyRequests.Track")]
        public IActionResult RequestTrack(int requestId)
        {
            return Ok(

           _requestManagement.GetRequestManagements(requestId,(int)ServiceTypeEnum.emergencyRequest)
           .Select(M => new
           {
               Id = M.Id,
               ProduceType = M.ProcedureType,
               date = M.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
               Attachments = M.Attachments.Select(n=>n.imagePath).ToList(),
               Notes = M.Notes,
               CreateBy = M.CreatedBy.FullName

           }).ToList()
            );
        }

        [HttpDelete]
        [Authorize(policy: "EmergencyRequests.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _emergencyService.Delete(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

    }
}
