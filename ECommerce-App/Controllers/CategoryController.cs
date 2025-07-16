using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(new
                {
                    data = categories,
                    status = StatusCodes.Status200OK,
                    message = "Successfully retrieved all categories"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("AddnewCategory")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            try
            {
                var category = new Category
                {
                    name = request.name,
                    description = request.description,
                    products = request.products
                };

                if (await _categoryService.CheckCategoryExists(category))
                {
                    return Conflict(new
                    {
                        status = StatusCodes.Status409Conflict,
                        message = "Category already exists"
                    });
                }

                await _categoryService.CreateAsync(category);
                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Category added successfully",
                    categoryId = category.id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error adding category: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Category ID is required"
                });
            }

            try
            {
                var deleteResult = await _categoryService.DeleteCategory(id);
                if (!deleteResult)
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error deleting category: {ex.Message}" });
            }
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDto request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Category ID is required"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            try
            {
                var category = new Category
                {
                    id = id,
                    name = request.name,
                    description = request.description,
                    products = request.products
                };

                if (await _categoryService.CheckUpdateCategoryExists(category))
                {
                    return Conflict(new
                    {
                        status = StatusCodes.Status409Conflict,
                        message = "Category already exists with these details"
                    });
                }

                var updateResult = await _categoryService.UpdateCategory(id, category);
                if (!updateResult)
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Category updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error updating category: {ex.Message}" });
            }
        }
    }
}