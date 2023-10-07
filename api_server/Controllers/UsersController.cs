using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ApiServer.Data;
using ApiServer.Repositories;

namespace ApiServer.Controllers {
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase {
        private readonly UserRepository _userRepository;
        public UsersController(UserRepository userRepository) {
            _userRepository = userRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignUpCred user) {
            await _userRepository.PostUser(user);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserSignInCred user) {
            var res = await _userRepository.FindUser(user);

            if (res == null) {
                return Unauthorized();
            }

            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim("id", res!.Id),
                new Claim("username", res.Username),
                new Claim("email", res.Email),
                new Claim("firstName", res.FirstName),
                new Claim("lastName", res.LastName),
                new Claim("profilePicPath", res.ProfilePicPath ?? "")
            }, "Cookies");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await Request.HttpContext.SignInAsync("Cookies", claimsPrincipal,  new AuthenticationProperties {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new LoggedInUserInfo { 
                Id= res.Id, 
                Username = res.Username, 
                FirstName = res.FirstName, 
                LastName = res.LastName 
            });
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync();

            return Ok();
        }
    }
}
