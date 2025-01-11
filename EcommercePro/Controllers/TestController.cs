using EtisiqueApi.Repositiories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtisiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private MessageSender2.IMessageSender _messageSender;
        public TestController(MessageSender2.IMessageSender messageSender) {

            _messageSender = messageSender;
        }
     [HttpGet ]
    [AllowAnonymous]
        public async Task<IActionResult> TestSMS(string message, string phones)
        {
 
            if (await _messageSender.Send3Async(phones, message + DateTime.UtcNow.Ticks.ToString(), null))
                return Ok();

            return BadRequest("Unable to send");
        }

    }
}
