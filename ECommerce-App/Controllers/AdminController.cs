using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;
namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpPost("Login")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login(LoginDTO credentials)
        {
            try
            {
                var admin = await _adminService.GetAdmin(credentials.Email);
                if (admin == null)
                {
                    return Unauthorized(new
                    {
                        status = 401,
                        Message = "Invalid email",
                    });
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(credentials.Password, admin.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized(new
                    {
                        status = 401,
                        Message = "Invalid password",
                    });
                }

                return Ok(new
                {
                    status = 200,
                    Message = "Login Successful",
                    data = admin
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    Message = "Login failed",
                    Error = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    Message = "An error occurred while processing your request",
                    Error = ex.Message,
                });
            }
        }
        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateAdmin([FromForm] AdminFormRequest request)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .ToList();

                //logger.LogError("Validation failed with {ErrorCount} errors: {@ValidationErrors}",
                //    errors.Count, errors);

                return BadRequest(
                    new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });
            }

            try
            {
                // Create admin entity
                var admin = new Admin
                {
                    Name = request.Name,
                    Username = request.Username,
                    Email = request.Email,
                    ContactNo = request.ContactNo,
                    Gender = request.Gender,
                    Content = request.Content,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                // Handle image upload
                if (request.ImageFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await request.ImageFile.CopyToAsync(memoryStream);

                    // Validate image size (e.g., max 5MB)
                    if (memoryStream.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest("Image size exceeds 5MB limit");
                    }

                    admin.ImageBytes = memoryStream.ToArray();
                }
                else if (!string.IsNullOrEmpty(request.ImageBase64))
                {
                    try
                    {
                        var base64Data = request.ImageBase64.Contains(",")
                            ? request.ImageBase64.Split(',')[1]
                            : request.ImageBase64;
                        var c = request.ImageBase64.Split(',')[0];
                        admin.Content = c;
                        admin.ImageBytes = Convert.FromBase64String(base64Data);
                    }
                    catch (FormatException)
                    {
                        return BadRequest("Invalid base64 image format");
                    }
                }

                await _adminService.CreateAsync(admin);

                return Ok(new
                {
                    Id = admin.Id,
                    Message = "Admin created successfully"
                });
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Error creating admin: {ErrorMessage}", ex.Message);
                return StatusCode(500, new
                {
                    Message = "An error occurred while creating admin",
                    Error = ex.Message
                });
            }
        }

        // Handle form data with file upload
    }
}
