using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUsers users;
        public UserController(IUsers users)
        {
            this.users = users;
        }
        [HttpPost("CreateUser")]
        [Consumes("application/json")]
        public async Task<IActionResult> createUser(CreateUserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { status = 300, message = "User Is Empty" });
            }
            var checkuser = users.GetUser(user.email);
            if (checkuser != null)
            {
                return BadRequest(new { statuscode = 401, message = "User Already Exists" });
            }
            var u = new User
            {
                name = user.name,
                contactno = user.contactno,
                gender = user.gender,
                email = user.email,
                password = BCrypt.Net.BCrypt.HashPassword(user.password),
                imageBytes = user.image,
                LocalAddress = user.localaddress,
            };
            await users.createUser(u);
            return Ok(new { status = 200, message = "User Created Successfully" });
        }
        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> LoginUser(UserLoginDTO userlogin)
        {
            if (userlogin == null || string.IsNullOrEmpty(userlogin.email) || string.IsNullOrEmpty(userlogin.password))
            {
                return BadRequest(new { status = 400, message = "User data is empty or incomplete" });
            }
            User user = await users.GetUser(userlogin.email);
            if (user == null)
            {
                return Unauthorized(new { status = 401, message = "Invalid username or password" });
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userlogin.password, user.password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { status = 401, message = "Invalid username or password" });
            }
            return Ok(new { status = 200, message = "User logged in successfully", data = user });
        }


    }
}
