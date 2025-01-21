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
        private IConfiguration configuration;
        public TestController(IConfiguration _configuration) {

            configuration = _configuration;
         }

        [HttpGet ]
        [AllowAnonymous]
        public  IActionResult GetLasVersion()
        {
            var Version = this.configuration.GetSection("AppVersion");
            return Ok(new {version = Version.Value});

        }

    }
}
