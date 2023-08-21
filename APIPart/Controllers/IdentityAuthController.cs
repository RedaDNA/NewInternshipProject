using APIPart.DTOs.AuthUserDtos;
using APIPart.DTOs.UserDtos;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities.identity;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityAuthController : Controller
    {
        IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityAuthController(IConfiguration configuration, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Auth(AuthUserDto authUserDto)
        {

            //   IActionResult response = Unauthorized();
            // ApiResponse resonse1 = response as ApiResponse;
            if (!ModelState.IsValid)
            {
                return new ApiOkResponse(ModelState);
            }
            var user = await _userManager.FindByNameAsync(authUserDto.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, authUserDto.Password))
            {
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                );

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                };

                var expires = DateTime.UtcNow.AddMinutes(2);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expires,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return new ApiOkResponse(jwtToken);
            }

            return new ApiResponse(401, "Unauthorized");
        }
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ApiResponse> SignUp(SignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiOkResponse(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = signUpDto.UserName,
                Email = signUpDto.Email,
                  PhoneNumber = signUpDto.PhoneNumber
                // Set other properties as needed
            };

            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (result.Succeeded)
            {
                return new ApiOkResponse("User created successfully");
            }
            else
            {
                // User creation failed, return appropriate error response
                var errors = result.Errors.Select(error => error.Description);
                return new ApiResponse(400, "User creation failed", errors);
            }
        }
    }
}