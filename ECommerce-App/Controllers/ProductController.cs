using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _productService;

        public ProductController(IProduct productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("Create")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if (productDto == null)
                return BadRequest("Product data is required");

            try
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = productDto.Quantity,
                    CategoryId = productDto.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Rating = 0
                };

                if (!string.IsNullOrEmpty(productDto.ImageBase64))
                {
                    var base64Parts = productDto.ImageBase64.Split(',');
                    if (base64Parts.Length != 2)
                        return BadRequest("Invalid base64 image format.");

                    var contentTypePart = base64Parts[0].Split(':')[1].Split(';')[0];
                    var base64Data = base64Parts[1];

                    product.ImageContentType = contentTypePart;
                    product.ImageExtension = $".{contentTypePart.Split('/')[1]}";
                    product.ImageBytes = Convert.FromBase64String(base64Data);
                }

                await _productService.CreateAsync(product);
                return Ok(new { Id = product.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating product: {ex.Message}");
            }
        }


        [HttpPut]
        [Consumes("application/json")]
        [Route("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, ProductCreateDto product)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Product ID is required");
            if (product == null)
                return BadRequest("Product data is required");
            else
            {
                var Product = new Product
                {
                    Id = id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    CategoryId = product.CategoryId,
                };
                if (!string.IsNullOrEmpty(product.ImageBase64))
                {
                    var base64Parts = product.ImageBase64.Split(',');
                    if (base64Parts.Length != 2)
                        return BadRequest("Invalid base64 image format.");
                    var contentTypePart = base64Parts[0].Split(':')[1].Split(';')[0];
                    var base64Data = base64Parts[1];
                    Product.ImageContentType = contentTypePart;
                    Product.ImageExtension = $".{contentTypePart.Split('/')[1]}";
                    Product.ImageBytes = Convert.FromBase64String(base64Data);
                }
                await _productService.updateProduct(id, Product);
                return Ok(new { 
                    status = 300,
                    message ="Product Updated Successfully"});
            }
        }
    }
}