using EcommercePro.DTO;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EtisiqueApi.DTO.RuleDto;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        IRuleService _ruleService;
        public RuleController(IRuleService ruleService) { 
        _ruleService= ruleService;

        }
        [HttpPost("Add")]
        [Authorize(policy: "Rules.manage")]
        public IActionResult Add(RuleDtoFoAdd ruleDto)
        {
            if (ModelState.IsValid)
            {
              var Result =  _ruleService.AddRules(ruleDto.Text, ruleDto.Projects);
                if(!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }
                return Ok();

            }
            return BadRequest();

        }
        [HttpGet("Public")]
        [AllowAnonymous]
        public IActionResult PublicRules()
        {
            List<string>PublicRules = _ruleService.GetpublicRules();
            Rule lastUpdate = _ruleService.getLastRule();

            return Ok(new
            {
                Rules = PublicRules,
                lastUpdated = lastUpdate?.LastUpdate
            });

        }
        [HttpGet("ProjectRules/{id}")]
        [Authorize]
        public IActionResult GetByProject(int id)
        {
           List<string> ProjectRules = _ruleService.GetRulesByProject(id);
            Rule lastUpdate = _ruleService.getLastRule();
            return Ok(new
            {
                Rules = ProjectRules,
                lastUpdated = lastUpdate?.LastUpdate
            });
        }
        [HttpGet("GetAll")]
        [Authorize(policy: "Rules.manage")]
        public IActionResult GetAll()
        {
            List<RuleForShow> ProjectRules = _ruleService.GetAllRules();
            Rule lastUpdate = _ruleService.getLastRule();
            return Ok(new
            {
                Rules = ProjectRules,
                lastUpdated = lastUpdate?.LastUpdate
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            Rule rule = await _ruleService.GetByIdAsync(id);
            return Ok(new
            {
                Rule = rule.Text,
                updateData=rule.LastUpdate.ToString("dd-MM-yyyy hh:mm tt"),
                projectId=rule.ProjectId,

            });
        }
        [HttpDelete("{textId}")]
        [Authorize(policy: "Rules.manage")]
        public async Task<IActionResult> Delete(int textId)
        {
             
           var result = _ruleService.Delete(textId);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(RuleDtoForUpdate dtoForUpdate)
        {
            
            var result = _ruleService.UpdateRules(dtoForUpdate.TextId,dtoForUpdate.Text);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }


    }
}
