using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using vue_netcore_chatroom.Models;
using vue_netcore_chatroom.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vue_netcore_chatroom.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;


        public UserController(ILogger<BaseController> logger, IConfiguration configuration, IUserService userService) : base(logger, configuration)
        {
            _userService = userService;

        }

        [HttpGet("SignInSso")]
        public async Task<IActionResult> SignInSso()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            // This method return current user if exists, otherwise create a new user
            var user = await _userService.CreateSsoUser(claimsPrincipal);

            return Ok(user);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();

            List<UserDto> userDtos = users
                .Select(u => UserDto.FromDbModel(u))
                .ToList();

            return Ok(userDtos);
        }

        [HttpGet("Me")]
        public async Task<IActionResult> GetMe()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var user = await _userService.GetUserByClaimsPrincipal(claimsPrincipal);

            return Ok(user);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto updatedUser)
        {
            var user = await _userService.UpdateUser(updatedUser);

            return Ok(user);
        }
    }
}
