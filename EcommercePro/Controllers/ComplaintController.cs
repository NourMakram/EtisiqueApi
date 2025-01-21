using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.FinancialConnections;
using System.Drawing.Printing;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {

        IComplaintService _complaintService;
        IRequestManagement _requestManagement;
        IAcountService _acountService;
        public ComplaintController(IComplaintService complaintService,IAcountService acountService, IRequestManagement requestManagement = null)
        {

            _complaintService = complaintService;
            _requestManagement = requestManagement;
            _acountService = acountService;
        }
        [HttpPost]
        [Authorize(policy: "Complaints.Add")]
        public async Task<IActionResult> Add(ComplaintDto.ComplaintDtoForAdd complaintDto)
        {
            if (ModelState.IsValid)
            {
 
                try
                {  
                    var result  = await _complaintService.Add(complaintDto);

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
        [HttpGet("{id}")]
        [Authorize(policy: "Complaints.Show")]
        //[AllowAnonymous]
        public IActionResult Get(int id)
        {
            return Ok(_complaintService.Get(id));
        }

        [HttpGet("{Page}/{PageSize}")]
        [Authorize(policy: "Complaints.view")]
         public async Task<IActionResult>  Requests(string userId,int Page=1, int PageSize= 10,string techniciId = null, int Code = 0,
         string Status = null, string ClientName = null, string ProjectName = null, int day = 0, int week = 0, int Year = 0, int Month = 0)
       {
            List<int> projects = _acountService.GetUserProjects(userId);
            if (projects.Count() > 0)
            {
                var Requests1 = await _complaintService.GetRequestToProjectsManager(projects, techniciId, Code, Status, ClientName, ProjectName, day, week, Year, Month)
                  .ToPaginatedListAsync(Page, PageSize);
               return Ok(Requests1);
            }

            var Requests =  await _complaintService.FilterRequestsBy(techniciId, Code, Status, ClientName, ProjectName, day, week, Year, Month)
                    .ToPaginatedListAsync(Page, PageSize);
            return Ok(Requests);

        }

        [HttpPost("Transfer")]
        [Authorize(policy: "Complaints.transfer")]
         public async Task<IActionResult> Transfer(TransferDto transferDto)
        {
            var result = await _complaintService.Transfer(transferDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("Close")]
        [Authorize(policy: "Complaints.close")]
        public async Task<IActionResult> Close(CommonPartsmanagmentDto closeDto)
        {
            
            var result = await _complaintService.Close(closeDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPost("PartialClose")]
        [Authorize(policy: "Complaints.PartialClose")]
         public async Task<IActionResult> PartialClose(CommonPartsmanagmentDto partialCloseDto)
        {
            
            var result = await _complaintService.PartialClose(partialCloseDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpGet("Track/{requestId}")]
        [Authorize(policy: "Complaints.Track")]
         public IActionResult Track(int requestId, int ServiceType = (int)ServiceTypeEnum.ComplaintService)
        {
            return Ok(

           _requestManagement.GetRequestManagements(requestId, (int)ServiceTypeEnum.ComplaintService)
           .Select(M => new
           {
               Id = M.Id,
               ProduceType = M.ProcedureType,
               date = M.CreatedDate.ToString("dd-MM-yyyy | hh:mm:ss tt"),
               Attachments =M.Attachments.Select(n => n.imagePath).ToList(),
               Notes = M.Notes,
               CreateBy = M.CreatedBy.FullName

           }).ToList()
            );
        }

        [HttpGet("Techincan/{techinchId}/{page}/{PageSize}")]
        [Authorize(policy: "Complaints.Technician")]
        public async Task<IActionResult> GetTechincianRequest(string techinchId, int page = 1, int PageSize = 10, string Status = null, int day = 0)
        {

            var Requests = await _complaintService.GetTechincanRequest(techinchId, Status, day).ToPaginatedListAsync(page, PageSize);

            return Ok(Requests);


        }
        [HttpDelete("{id}")]
        [Authorize(policy: "Complaints.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var reuslt = await _complaintService.Delete(id);

            if (!reuslt.Succeeded)
            {
                return BadRequest(reuslt.Errors);
            }
            return Ok();


        }


         



    }
}
