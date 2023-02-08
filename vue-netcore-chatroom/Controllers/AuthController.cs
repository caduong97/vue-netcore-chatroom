using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vue_netcore_chatroom.Models;
using vue_netcore_chatroom.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vue_netcore_chatroom.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(ILogger<BaseController> logger, IConfiguration configuration, IAuthService authService) : base(logger, configuration)
        {
            _authService = authService;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Register request is not valid");
            }

            AuthenticationResponse response = await _authService.SignIn(request);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterPasswordUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Register request is not valid");
            }

            AuthenticationResponse response = await _authService.Register(request);

            return Ok(response);
        }
    }
}

