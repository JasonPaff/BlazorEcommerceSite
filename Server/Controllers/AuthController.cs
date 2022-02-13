using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // inject auth service
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // register user route
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister request)
        {
            // register user
            var response = await _authService.Register(
                new User
                {
                    Email = request.Email
                }, request.Password);

            // register failed
            if (!response.Success) return BadRequest(response);
            
            // register succeeded
            return Ok(response);
        }
        
        // login user route
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin request)
        {
            var response = await _authService.Login(request.Email, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // change user password, must be authorized
        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string newPassword)
        {
            // get user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // change password
            var response = await _authService.ChangePassword(int.Parse(userId), newPassword);

            // failed
            if (!response.Success)
                return BadRequest(response);

            // succeeded
            return Ok(response);
        }
    }
}