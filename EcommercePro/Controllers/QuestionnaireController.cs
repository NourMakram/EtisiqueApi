using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireController : ControllerBase
    {
        IQuestionnaireService _questionnaireService;
        IShortCutService _ShortService;
        IAcountService _acountService;
        public QuestionnaireController(IQuestionnaireService questionnaireService,IAcountService acountService ,IShortCutService ShortService) {
        
           _questionnaireService = questionnaireService;
            _ShortService = ShortService;
            _acountService = acountService;
        
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(QuestionnaireDto questionnaireDto)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var IsReted = _questionnaireService.IsRated(questionnaireDto.RequestId, questionnaireDto.ServiceType);
                    if (IsReted)
                    {
                        return BadRequest("the request is Rated");
                    }
                    var IsRequester = _questionnaireService.IsRequester(questionnaireDto.RequestId, questionnaireDto.ServiceType, questionnaireDto.CustomerId);
                    if (!IsRequester)
                    {
                        return BadRequest("Can not rate this Request");
                    }

                    var Result = await _questionnaireService.Add(questionnaireDto);
                    if (!Result.Succeeded)
                    {
                        return BadRequest(Result.Errors);
                    }
                    return Ok();

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);

                }

            }
            return BadRequest();

        }
        [HttpGet("Service/{page}/{pageSize}")]
        [Authorize(Policy = "Questionnaires.ServicesView")]
        public async Task<IActionResult> GetEvelautionService(string UserId, int page =1,int pageSize=10,int RequestCode=0 , string ClientName=null,int Year=0 , int Month=0 , int Week=0, string ProjectName=null,int rating=0)
       {
            List<int> projects = _acountService.GetUserProjects(UserId);
            if(projects.Count() > 0)
            {
                var Result1 = await _questionnaireService.GetRequestToProjectsManager(projects, RequestCode, Week, Year, Month, ClientName, ProjectName,rating)
               .ToPaginatedListAsync(page, pageSize);
               return Ok(Result1);
            }
            var Result =await _questionnaireService.GetEvelautionsService(RequestCode, Week, Year, Month, ClientName,ProjectName,rating)
                .ToPaginatedListAsync(page,pageSize);
            return Ok(Result);
        }
        [HttpGet("Technician")]
        [Authorize(Policy = "Questionnaires.TechniciansView")]
        public IActionResult GetEvelautionTechnician(string UserId,string TechnicianId=null, int Year=0, int Month=0, int Week=0, DateOnly from=default ,DateOnly to=default, string ProjectName = null)
        {
            List<int> projects = _acountService.GetUserProjects(UserId);
            if (projects.Count() > 0)
            {
            var Result1 = _questionnaireService.GetEvelautionTechnicianToProjectManager(projects,TechnicianId, Week, Year, Month, from, to, ProjectName);
                return Ok(Result1);

            }
            var Result = _questionnaireService.GetEvelautionTechnician(TechnicianId, Week, Year, Month,from,to,ProjectName);
               
            return Ok(Result);
        }
         
        [HttpPut("Note")]
        [Authorize(Policy = "Questionnaires.Add")]
        public async Task<IActionResult> AddNote(EvelautionServiceNoteDto noteDto)
        {
            if (ModelState.IsValid)
            {
                var result =await  _questionnaireService.AddNote(noteDto.Id, noteDto.Note);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();

            }
            return BadRequest();

        }

        [HttpGet("Url/{Id}")]
        [AllowAnonymous]
        public IActionResult GetUrl(string Id)
        {
            Shrt shortCut = _ShortService.get(Id);
            return Ok(shortCut);
        }

    }
}
