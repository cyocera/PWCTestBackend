using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PWCExamService.Data;
using PWCExamService.Managers;

namespace PWCExamService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager services;
        public AuthController(IAuthManager _services)
        {
            services= _services;
        }

        [HttpPost, Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserEntity request)
        {
            var response = await services.Login(request);
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost, Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserEntity request)
        {
            var response = await services.Register(request);
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
