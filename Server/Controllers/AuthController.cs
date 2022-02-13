﻿using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}