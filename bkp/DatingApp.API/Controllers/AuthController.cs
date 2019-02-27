using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config,IMapper mapper)
        {
            this._mapper = mapper;
            this._config = config;
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if(await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("UserName already exists");
            
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var CreatedUser = await _repo.Register(userToCreate,userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(CreatedUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = CreatedUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo =await _repo.Login(userForLoginDto.Username,userForLoginDto.Password);
            if(userFromRepo == null)
                return Unauthorized();
            
            var claims = new [] 
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);
            return Ok( new {
                    token = tokenHandler.WriteToken(token),
                     user
            });



        }
        
    }
}