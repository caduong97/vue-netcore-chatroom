using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using vue_netcore_chatroom.Helpers;
using vue_netcore_chatroom.Models;

namespace vue_netcore_chatroom.Services
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> Register(RegisterPasswordUserRequest request);
        Task<AuthenticationResponse> SignIn(AuthenticationRequest request);

    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthService(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<AuthenticationResponse> Register(RegisterPasswordUserRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);

            if (user != null)
            {
                throw new Exception("User with the same already exists");
            }

            user = await _userService.CreatePasswordUser(request);

            var accessToken = GetAccessToken(user);

            return new AuthenticationResponse(accessToken);

        }

        public async Task<AuthenticationResponse> SignIn(AuthenticationRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);

            if (user == null || String.IsNullOrWhiteSpace(user.HashedPassword))
            {
                throw new Exception("User does not exist");
            }

            var passwordVerified = PasswordHasher.Verify(request.Password, user.HashedPassword);

            if (!passwordVerified)
            {
                throw new Exception("Email or password is not correct");
            }


            var accessToken = GetAccessToken(user);

            return new AuthenticationResponse(accessToken);

        }


        private string GetAccessToken(User user)
        { 
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["CustomJwtAuth:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.AuthenticationMethod, "CustomJwt")
            };

            var token = new JwtSecurityToken(
                _configuration["CustomJwtAuth:Issuer"],
                _configuration["CustomJwtAuth:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }

        

        
    }
}
