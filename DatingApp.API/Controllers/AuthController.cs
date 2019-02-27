using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public UserManager<User> _userManager { get; }

        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config,IMapper mapper,
        UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this._mapper = mapper;
            this._config = config;
            //this._repo = repo;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if(await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("UserName already exists");
            
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

           // var CreatedUser = await _repo.Register(userToCreate,userForRegisterDto.Password);
            var result = await _userManager.CreateAsync(userToCreate,userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(result);
            if(result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new {controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user =await _userManager
                            .FindByNameAsync(userForLoginDto.Username);
            var result =await _signInManager
                            .CheckPasswordSignInAsync(user,userForLoginDto.Password,false);

            if (result.Succeeded)
            {
                var userApp = await _userManager.Users.Include(p => p.Photos)
                                .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

                var userToreturn = _mapper.Map<UserForListDto>(userApp);
                return Ok( new {
                        token = GenerateJwtToken(userApp).Result,
                        user = userToreturn
                    });
            }
 
            return Unauthorized();

           
        }

        private async Task<string> GenerateJwtToken(User user)
        {
             var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

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

            return tokenHandler.WriteToken(token);

        }
        
    }
}