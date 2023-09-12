using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ApiServer.Data;
using ApiServer.Services;

namespace ApiServer.Controllers {
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase {
        private readonly UserService _userService;
        public UsersController(UserService userService) {
            _userService = userService;
        }

        [HttpPost("/api/signup")]
        public async Task<IActionResult> SignUp(UserSignUpCred user) {
            try {
                await _userService.AddUser(user);

                return Ok();
            } catch (Exception ex) {
                 Console.WriteLine(ex.Message);
                 
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login(UserSignInCred user) {
            try {
                var res = await _userService.CheckCredentials(user);

                var claimsIdentity = new ClaimsIdentity(new[] {
                    new Claim("id", res!.Id),
                    new Claim("username", res.Username),
                    new Claim("profilePicPath", res.ProfilePicPath ?? "")
                }, "Cookies");

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await Request.HttpContext.SignInAsync("Cookies", claimsPrincipal,  new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                });

                return Ok(new LoggedInUserInfo { Id= res.Id, Username = res!.Username });
            } catch (Exception ex) {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("/api/logout")]
        public async Task<IActionResult> Logout() {
            try {
                await HttpContext.SignOutAsync();

                return Ok();
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
