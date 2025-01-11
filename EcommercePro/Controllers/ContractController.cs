using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private IContractService _contractService;
        private IFileService _fileService;
        public ContractController(IContractService contractService, IFileService fileService)
        {
            _contractService = contractService;
            _fileService = fileService;

        }
        [HttpPost]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Add([FromForm] ContractDto contractDto)
        {
            if (ModelState.IsValid)
            {
                if (contractDto.FormFileUrl != null)
                {
                    var result = await _fileService.SaveImage("Contract", contractDto.FormFileUrl);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);

                    }
                    contractDto.contractFile = result.imagePath;
                }

                var ResultOfAdd = await _contractService.AddAsync(new EcommercePro.Models.Contract()
                {
                    contractFile = contractDto.contractFile,
                    StartDate = contractDto.StartDate,
                    EndDate = contractDto.EndDate,
                    projectId = contractDto.projectId,
                    description = contractDto.description,
                    TcvIncTax = contractDto.TcvIncTax,
                    Status = 1


                });
                if (!ResultOfAdd.Succeeded)
                {
                    return BadRequest(ResultOfAdd.Errors);
                }
                return Ok();
            }
            return BadRequest(ModelState);

        }
        [HttpPut("{contractId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Update(int contractId, [FromForm] ContractDto contractDto)
        {
            if (ModelState.IsValid)
            {
                Contract contractDb = await _contractService.GetByIdAsync(contractId);
                if (contractDb == null)
                {
                    return BadRequest("Contract Not Found");

                }
                if (contractDto.FormFileUrl != null)
                {
                    var result = await _fileService.ChangeImage("Contract", contractDto.FormFileUrl, contractDb.contractFile);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                    contractDb.contractFile = result.imagePath;

                }
                

                contractDb.StartDate = contractDto.StartDate;
                contractDb.EndDate = contractDto.EndDate;
                contractDb.projectId = contractDto.projectId;
                contractDb.description = contractDto.description;
                contractDb.TcvIncTax = contractDto.TcvIncTax;
                contractDb.Status = contractDto.Status;

                var ResultOfUpdate = _contractService.Update(contractDb);
                if (!ResultOfUpdate.Succeeded)
                {
                    return BadRequest(ResultOfUpdate.Errors);
                }
                return Ok();

            };
            return BadRequest(ModelState);
        }
        [HttpDelete("Delete/{contractId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> Delete(int contractId)
        {
            Contract contractDb = await _contractService.GetByIdAsync(contractId);
            if (contractDb == null)
            {
                return BadRequest("Contract Not Found");

            }
            var ResultOfDelete = _contractService.Delete(contractDb);
            if (!ResultOfDelete.Succeeded)
            {
                return BadRequest(ResultOfDelete.Errors);
            }
            return Ok();

        }
        [HttpDelete("SoftDelete/{contractId}")]
        [Authorize(policy: "projects.manage")]
        public async Task<IActionResult> SoftDelete(int contractId)
        {
            Contract contractDb = await _contractService.GetByIdAsync(contractId);
            if (contractDb == null)
            {
                return BadRequest("Contract Not Found");

            }
            contractDb.IsDeleted = true;
            var ResultOfDelete = _contractService.Update(contractDb);
            if (!ResultOfDelete.Succeeded)
            {
                return BadRequest(ResultOfDelete.Errors);
            }
            return Ok();

        }

        [HttpGet("{ProjectId}")]
        [Authorize(policy: "projects.view")]
        public async Task<IActionResult> GetContract(int ProjectId)
        {
            ContractResponse contract = await _contractService.GetByProjectId(ProjectId);

            return Ok(contract);

        }
        [HttpGet("get/{id}")]
        [Authorize(policy: "projects.view")]
        public async Task<IActionResult> Get(int id)
        {
            Contract contract = await _contractService.GetByIdAsync(id);
            ContractResponse contractData = null;
            if (contract != null)
            {
                contractData = new ContractResponse()
                {
                    Id = contract.Id,
                    FilePath = contract.contractFile,
                    SatrtDate = contract.StartDate.ToString("yyy-MM-dd"),
                    EndDate = contract.EndDate.ToString("yyy-MM-dd"),
                    Description = contract.description,
                    stuats = contract.Status.ToString(),
                    TotalContracValue = contract.TcvIncTax.ToString(),
                    ProjectId = contract.projectId

                };

            }

            return Ok(contractData);

        }
    }
}
