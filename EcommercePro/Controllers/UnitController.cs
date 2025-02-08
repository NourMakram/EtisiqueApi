using EcommercePro.Models;
using EcommercePro.Repositiories.Interfaces;
using EtisiqueApi.Repositiories.Interfaces;
using EtisiqueApi.Repositiories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EtisiqueApi.DTO;
using EtisiqueApi.Pagination;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        
        private ICustomerService _customerService;
        
        public UnitController(ICustomerService customerService)
        {
            _customerService = customerService;  
        }
        [HttpGet("UnitsManagment/{page}/{pagesize}/{projectId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> UnitsManagment(int projectId,int page=1 , int pagesize=10, 
            string buidingName=null, string ClientName=null,
            string ClientPhone=null, DateOnly from=default, DateOnly to=default)
        {

        var UnitsManagment = await  _customerService.UnitsManagement(projectId, buidingName, ClientName, ClientPhone, from, to)
                .ToPaginatedListAsync(page,pagesize);
            return Ok(UnitsManagment);
        }
        [HttpPut]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Update(UnitsManagmentDto unitsManagmentDto)
        {

            var result = await _customerService.UpdateUnitsManagement(unitsManagmentDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpGet("Check/{userId}/{projectId}/{unitNo}/{buidingName}")]
        [Authorize]
        public IActionResult CheckGuarantee(string userId, int projectId, int unitNo, string buidingName)
        {
            bool result = _customerService.IsGuaranteeAvailable(userId,projectId,unitNo,buidingName);
            return Ok(new { result = result });

        }
        [HttpGet("Get/{Id}")]
        [Authorize]
        public async Task<IActionResult> GetUnit(int Id)
        {
            var Unit =await  _customerService.GetByIdAsync(Id);
            if (Unit != null)
            {
                var result = new Client()
                {

                    Id = Unit.Id.ToString(),
                    userId = Unit.UserId,
                    FullName = Unit.ApplicationUser.FullName,
                    Email = Unit.ApplicationUser.Email,
                    phoneNumber = Unit.ApplicationUser.PhoneNumber,
                    UnitNum = Unit.UnitNo,
                    BuildingName = Unit.BulidingName,
                    GuaranteeStart = Unit.GuaranteeStart,
                    GuaranteeEnd = Unit.GuaranteeEnd,
                    project = Unit.ApplicationUser.Project.ProjectName,
                    ReceivedDate = Unit.ReceivedDate != null ? Unit.ReceivedDate.Value.ToString("dd-MM-yyyy hh:mm tt") : "No date available"


                };
                return Ok(result);

            }
            return Ok(Unit);

        }

        //[HttpGet("change/{Id}")]
        //[AllowAnonymous]
        //public IActionResult cchange(int Id)
        //{
        //    var result =_customerService.delete(Id);
        //    return Ok(result);
        //}


    }
}
