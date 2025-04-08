using BlogAppWebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BlogAppWebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BlogAppWebApi.Data;

namespace BlogAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BlogAppDbContext dbContext;

        private readonly IPasswordHasher<User> passwordHasher;

        public UsersController(BlogAppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objRegister = dbContext.User.FirstOrDefault(x => x.Email == registerDto.Email);
            if (objRegister == null)
            {
                var newUser = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email
                };
                newUser.Password = passwordHasher.HashPassword(newUser, registerDto.Password);
                dbContext.User.Add(newUser);
                await dbContext.SaveChangesAsync();

                return Ok("User registered success");
            }
            else
            {
                return BadRequest("User already existx with the same email adress");

            }
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = dbContext.User.FirstOrDefault(x => x.Email == loginDto.Email);
            
            if (user != null)
            { 

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Email, user.Email)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok("Login success");
            }

            return Unauthorized("Invalid email or password");
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }

    }
}
