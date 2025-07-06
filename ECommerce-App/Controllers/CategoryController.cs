using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        [Route("getAllCategories")]
        [Consumes("application/json")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(new { data = categories, status = 200, message = "Gett All Categories Data" });
        }
        [HttpPost]
        [Route("AddnewCategory")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = new Category
            {
                name = request.name,
                description = request.description,
                products = request.products
            };
            var check = await _categoryService.CheckCategoryExists(category);
            if (check == false)
            {
                return BadRequest(new { status = 400, message = "Category Already Exists" });
            }
            await _categoryService.CreateAsync(category);
            return Ok(new { status = 200, message = "Category Added Successfully" });
        }
        [HttpDelete]
        [Route("DeleteCategory/{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { status = 400, message = "Category ID is required" });
            }

            var delete = await _categoryService.DeleteCategory(id);
            if(!delete)
            {
                return NotFound(new { status = 404, message = "Category not found" });
            }
            return Ok(new { status = 200, message = "Category deleted successfully" });
        }
        [HttpPut]
        [Route("UpdateCategory/{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDto request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { status = 400, message = "Category ID is required" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = new Category
            {
                id = id,
                name = request.name,
                description = request.description,
                products = request.products
            };
            var check = await _categoryService.CheckUpdateCategoryExists(category);
            if (check == false)
            {
                return BadRequest(new { status = 400, message = "Category Already Exists" });
            }
            var updateResult = await _categoryService.UpdateCategory(id, category);
            if (!updateResult)
            {
                return NotFound(new { status = 404, message = "Category not found" });
            }
            return Ok(new { status = 200, message = "Category updated successfully" });
        }

    }
}
