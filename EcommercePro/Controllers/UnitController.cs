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
        private ILocationService _locationService;
        public UnitController(ICustomerService customerService,ILocationService locationService)
        {
            _customerService = customerService;
            _locationService = locationService;
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
        [HttpGet("CheckGuarantee/{userId}/{projectId}/{unitNo}/{buidingName}")]
        [Authorize]
        public IActionResult CheckGuarantee(string userId, int projectId, int unitNo, string buidingName)
        {
            bool result = _customerService.IsGuaranteeAvailable(userId,projectId,unitNo,buidingName);
            return Ok(new { result = result });

        }
        [HttpGet("CheckGuarantee/{Id}")]
        [Authorize]
        public IActionResult CheckGuarantee(int Id)
        {
            bool result = _customerService.IsGuaranteeAvailable(Id);
            return Ok(new { result = result });

        }
        [HttpGet("GetUnit/{Id}")]
        [Authorize]
        public async Task<IActionResult> GetUnitDeatils(int Id)
        {
            var Unit =await  _customerService.GetUnitDetails(Id);
            
            return Ok(Unit);

        }
        [HttpGet("UnitsWithGuarantee/{userId}")]
        [Authorize]
        public IActionResult GetUnitsWithGuarantee(string userId)
        {
            List<unit> units = _customerService.UnitsWithGuarantee(userId);
            return Ok(units);
        }

        [HttpGet("Get/{Id}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetUnit2(int Id)
		{
			var Unit = await _customerService.GetByIdAsync(Id);
            if (Unit == null)
            {
                return BadRequest("Unit Not Found");
            }
               Location location = _locationService.GetLocationByUserId(Unit.UserId);

            UnitDetails unit = new UnitDetails()
            {
                
                id = Unit.Id,
                FullName = Unit.ApplicationUser.FullName,
                Email = Unit.ApplicationUser.Email,
                PhoneNumber = Unit.ApplicationUser.PhoneNumber,
                projectId=Unit.projectId != null ? (int)Unit.projectId : 0,
                //projectId = (int)Unit.ApplicationUser.projectId != null ?(int)Unit.ApplicationUser.projectId : 0,
                TypeProject = Unit.TypeProject, //مالك - مستأجر
                City = Unit.City,
                UnitNo = Unit.UnitNo,
                Latitude =(double) location?.Latitude,
                Longitude = (double)location?.Longitude,
                UserId = Unit.UserId,
                BuidingName = Unit.BulidingName,
                GuaranteeStart = Unit.GuaranteeStart,
                GuaranteeEnd = Unit.GuaranteeEnd,

            };

                return Ok(unit);

		}
        [HttpPost]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult>Add (NewUnit newUnit)
        {
            var result = await _customerService.AddUnit(newUnit);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPut("UpdateUnit")]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult> Update(NewUnit newUnit)
        {
            var result = await _customerService.UnpdateUnit(newUnit);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpGet("HajarUnit/{Id}")]
        [Authorize]
        public IActionResult HajarProject(int Id)
        {
            var result = _customerService.IsHajarPRojct(Id);
            return Ok(new { reuslt = result });
        }


        [HttpGet("Units/{userId}")]
        public IActionResult UnitsByUserId(string userId)
        {
          List<unit> units= _customerService.GetUnitsByUserId(userId);
            return Ok(units);
        }
        [HttpDelete]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var Unit = await _customerService.GetByIdAsync(id);
            if (Unit == null)
            {
                return BadRequest("Unit Not Found");
            }
            var result = _customerService.Delete(Unit);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }




    }
}
