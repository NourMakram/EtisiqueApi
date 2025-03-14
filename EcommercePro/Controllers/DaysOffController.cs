using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaysOffController : ControllerBase
    {
        IGenaricService<DaysOff> _DaysOffService;
        public DaysOffController(IGenaricService<DaysOff> DaysOffService) {
        
        _DaysOffService = DaysOffService;   
        }
        [HttpPost]
        [Authorize(policy: "DaysOff.manage")]
        public async Task<IActionResult> Add(DaysOff daysOff)
        {
            if (ModelState.IsValid)
            {
                var result = await _DaysOffService.AddAsync(daysOff);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            return BadRequest();

        }
        [HttpPut]
        [Authorize(policy: "DaysOff.manage")]
        public async Task<IActionResult> Edit(DaysOff daysOff)
        {
            if (ModelState.IsValid)
            {
                DaysOff dayoff = await _DaysOffService.GetByIdAsync(daysOff.Id);
                if (dayoff == null)
                {

                    return BadRequest("Not Found");

                }
                dayoff.From = daysOff.From;
                dayoff.To = daysOff.To;
                var result = _DaysOffService.Update(dayoff);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Authorize(policy: "DaysOff.manage")]
        public IActionResult GetAll()
        {
            List<DaysOff> daysOffs = _DaysOffService.GetAll();
            return Ok(daysOffs);
        }

        [HttpGet("{id}")]
        [Authorize(policy: "DaysOff.manage")]
        public async Task<IActionResult> Get(int id)
        {
            DaysOff dayoff = await _DaysOffService.GetByIdAsync(id);
            if (dayoff == null)
            {

                return BadRequest("Not Found");

            }
            return Ok(dayoff);

        }
        [HttpDelete]
        [Authorize(policy: "DaysOff.manage")]
        public async Task<IActionResult> Delete(int id)
        {
            DaysOff dayoff = await _DaysOffService.GetByIdAsync(id);
            if (dayoff == null)
            {

                return BadRequest("Not Found");

            }
            var result = _DaysOffService.Delete(dayoff);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpGet("daysoff")]
        [AllowAnonymous]
        public IActionResult GetDaysoff()
        {
            List<string> daysoff = new List<string>();
            List<DaysOff> daysOffs = _DaysOffService.GetAll();
            foreach (DaysOff dayoff in daysOffs)
            {
                DateOnly from = dayoff.From;
                DateOnly to = dayoff.To;

                // Initialize the date variable to 'from'
                for (DateOnly date = from; date <= to; date = date.AddDays(1)) // Fix here
                {
                   if(!daysoff.Contains(date.ToString()))
                    {
                        daysoff.Add(date.ToString("dd/MM/yyyy"));

                    }
                }
            }
            return Ok(daysoff);


        }


    }
}
