using APIPart.DTOs.CarDtos;
using APIPart.DTOs.UserDtos;
using APIPart.ErrorHandling;
using AutoMapper;
using Azure;
using Core.Entities;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
       IConfiguration _configuration;
        private readonly IMapper _mapper;
        private IUserService _userService;
        public AuthController(IConfiguration configuration,IMapper mapper, IUserService userService)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost]
        public async IActionResult Auth(AuthUserDto authUserDto)
        {

            IActionResult response = Unauthorized();
            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }
            var userExists = await _userService.IsExistByUsernamePasswordAsync(authUserDto.UserName, authUserDto.Password);
                if(userExists)

                {

                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha512Signature
                                    );
                var subject = new ClaimsIdentity(new[]
{
new Claim(JwtRegisteredClaimNames.Sub, authUserDto.UserName),
new Claim(JwtRegisteredClaimNames.Email, authUserDto.UserName),
});
                var expires = DateTime.UtcNow.AddMinutes(2);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                return Ok(jwtToken);
            }

            return response;

        }

        [HttpPost("SignUp")]
        public async Task<ApiResponse> SignUp( CreateUserDto signUpUserDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            User toCreateUser = _mapper.Map<User>(signUpUserDto);
            try
            {
                var createdUser = await _userService.AddAsync(toCreateUser);
                var createdUserDto = _mapper.Map<UserDto>(createdUser);
                return new ApiOkResponse(createdUserDto);
            }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }

        }

    }
}
