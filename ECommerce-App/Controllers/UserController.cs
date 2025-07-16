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
                return BadRequest(new { status = 300 , message ="User Is Empty"});
            }
            var u = new User
            {
                name = user.name,
                contactno = user.contactno,
                gender = user.gender,
                email= user.email,
                password = BCrypt.Net.BCrypt.HashPassword(user.password),
                LocalAddress = user.localaddress,
            };
            await users.createUser(u);
            return Ok(new { status = 300, message = "User Created Successfully" });
        }

    }
}
