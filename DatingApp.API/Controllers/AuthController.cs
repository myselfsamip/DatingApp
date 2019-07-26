using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
        public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }
         [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDtos userForRegisterDtos)
        {
            //validation
            userForRegisterDtos.UserName = userForRegisterDtos.UserName.ToLower();
            if(await _repo.UserExists(userForRegisterDtos.UserName))
            return BadRequest("Username already exist");

            var userToCreate = new User()
            {
                UserName = userForRegisterDtos.UserName
            };

            var createdUser = await _repo.Register(userToCreate,userForRegisterDtos.PassWord);
            return StatusCode(201);


        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDtos userLoginDtos)
        {
        var userFromRepo = await _repo.Login(userLoginDtos.UserName.ToLower(),userLoginDtos.PassWord);
        if(userFromRepo == null){
            return Unauthorized();

        }
        var claims =new []
        {
            new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name,userFromRepo.UserName)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
       
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        var tokenDiscriptor = new SecurityTokenDescriptor{

            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds

        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDiscriptor);

        return Ok(new { token = tokenHandler.WriteToken(token)});
        }
    }
}