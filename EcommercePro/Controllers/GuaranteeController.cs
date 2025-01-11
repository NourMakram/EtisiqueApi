using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteeController : ControllerBase
    {
        private IGuaranteeService _GuaranteeService;
        private IFileService _fileService;
        public GuaranteeController(IGuaranteeService GuaranteeService, IFileService fileService)
        {
            _GuaranteeService = GuaranteeService;
            _fileService = fileService;

        }
        [HttpPost]
       // [Authorize(policy: "Guarantees.manage")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Add([FromForm] GuaranteeDto  guaranteeDto)
        {
            if (ModelState.IsValid)
            {
                if (guaranteeDto.formFileUrl != null)
                {
                    var result = await _fileService.SaveImage("Guarantee", guaranteeDto.formFileUrl);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);

                    }
                    guaranteeDto.FileUrl= result.imagePath;
                }

                var ResultOfAdd = await _GuaranteeService.AddAsync(new Models.Guarantee
                {
                    FileUrl = guaranteeDto.FileUrl,
                     EndDate = guaranteeDto.EndDate,
                     StartDate=guaranteeDto.StartDate,
                    projectId = guaranteeDto.projectId,
                    Notes = guaranteeDto.Notes,
                     GuaranteeType=guaranteeDto.GuaranteeType,
                    InsideApartment=guaranteeDto.InsideApartment,
                    Status=1,

                });
                if (!ResultOfAdd.Succeeded)
                {
                    return BadRequest(ResultOfAdd.Errors);
                }
                return Ok();
            }
            return BadRequest(ModelState);

        }
       
        [HttpPut("{guaranteeId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Update(int guaranteeId, [FromForm] GuaranteeDto guaranteeDto)
        {
            if (ModelState.IsValid)
            {
                Guarantee guaranteetDb = await _GuaranteeService.GetByIdAsync(guaranteeId);
                if (guaranteetDb == null)
                {
                    return BadRequest("Guarantee Not Found");

                }
                if (guaranteeDto.formFileUrl != null)
                {
                    var result = await _fileService.ChangeImage("Guarantee", guaranteeDto.formFileUrl, guaranteetDb.FileUrl);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                    guaranteeDto.FileUrl = result.imagePath;

                }
                else
                {
                    guaranteeDto.FileUrl = guaranteetDb.FileUrl;
                }

                guaranteetDb.FileUrl = guaranteeDto.FileUrl;
                 guaranteetDb.EndDate = guaranteeDto.EndDate;
                guaranteetDb.StartDate = guaranteeDto.StartDate;

                guaranteetDb.projectId = guaranteeDto.projectId;
                guaranteetDb.Notes = guaranteeDto.Notes;
                 guaranteetDb.GuaranteeType = guaranteeDto.GuaranteeType;
                guaranteetDb.InsideApartment = guaranteeDto.InsideApartment;
                guaranteetDb.Status = guaranteeDto.Status;

                var ResultOfUpdate = _GuaranteeService.Update(guaranteetDb);
                if (!ResultOfUpdate.Succeeded)
                {
                    return BadRequest(ResultOfUpdate.Errors);
                }
                return Ok();

            };
            return BadRequest(ModelState);
        }
       
        [HttpDelete("SoftDelete/{guaranteeId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> SoftDelete(int guaranteeId)
        {
            Guarantee GuaranteeDb = await _GuaranteeService.GetByIdAsync(guaranteeId);
            if (GuaranteeDb == null)
            {
                return BadRequest("Guarantee Not Found");

            }
            GuaranteeDb.IsDeleted = true;
            var ResultOfDelete = _GuaranteeService.Update(GuaranteeDb);
            if (!ResultOfDelete.Succeeded)
            {
                return BadRequest(ResultOfDelete.Errors);
            }
            return Ok();

        }
        
        [HttpDelete("Delete/{guaranteeId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Delete(int guaranteeId)
        {
            Guarantee GuaranteeDb = await _GuaranteeService.GetByIdAsync(guaranteeId);
            if (GuaranteeDb == null)
            {
                return BadRequest("Guarantee Not Found");

            }
            var ResultOfDelete = _GuaranteeService.Delete(GuaranteeDb);
            if (!ResultOfDelete.Succeeded)
            {
                return BadRequest(ResultOfDelete.Errors);
            }
            return Ok();

        }

        [HttpGet("{ProjectId}")]
        [Authorize(policy: "projects.view")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuarantee(int ProjectId)
        {
            return Ok(await _GuaranteeService.GetByProjectIdAsync(ProjectId));

        }
        [HttpGet("Get/{id}")]
        [Authorize(policy: "projects.view")]
       // [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
           Guarantee guaranteeDb =  await _GuaranteeService.GetByIdAsync(id);
            GuaranteeResponse guarantee = null;
            if (guaranteeDb != null)
            {
                 guarantee  = new GuaranteeResponse()
                {
                    Id = guaranteeDb.Id,
                    Notes = guaranteeDb.Notes,
                    EndDate = guaranteeDb.EndDate.ToString("yyyy-MM-dd"),
                    StartDate = guaranteeDb.StartDate.ToString("yyyy-MM-dd"),
                     FilePath = guaranteeDb.FileUrl,
                    Status = guaranteeDb.Status.ToString(),
                    GuaranteeType = guaranteeDb.GuaranteeType.ToString(),
                    InsideApartment = (guaranteeDb.InsideApartment) ? "true" : "false",
                    projectId = guaranteeDb.projectId
                };
            }
            return Ok(guarantee);

        }

    }
}
