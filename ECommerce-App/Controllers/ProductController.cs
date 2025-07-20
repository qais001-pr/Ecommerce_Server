using ECommerce_App.Model;
using ECommerce_App.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace ECommerce_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
          IProduct productService,
          ICategoryService categoryService,
          ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            _logger.LogInformation("Creating new product");

            if (productDto == null)
            {
                _logger.LogWarning("CreateProduct called with null DTO");
                return BadRequest("Product data is required");
            }

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
                    _logger.LogDebug("Processing product image");
                    var base64Parts = productDto.ImageBase64.Split(',');
                    if (base64Parts.Length != 2)
                    {
                        _logger.LogWarning("Invalid image format");
                        return BadRequest("Invalid base64 image format.");
                    }

                    try
                    {
                        var contentTypePart = base64Parts[0].Split(':')[1].Split(';')[0];
                        var base64Data = base64Parts[1];

                        product.ImageContentType = contentTypePart;
                        product.ImageExtension = $".{contentTypePart.Split('/')[1]}";
                        product.ImageBytes = Convert.FromBase64String(base64Data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing product image");
                        return BadRequest("Invalid image data format");
                    }
                }

                await _productService.CreateAsync(product);
                await _categoryService.UpdateProductCountByCategory(product.CategoryId);

                _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
                return Ok(new { Id = product.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating product");
            }
        }

        [HttpPut("Update/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductCreateDto productDto)
        {
            _logger.LogInformation("Updating product {ProductId}", id);

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("UpdateProduct called with empty ID");
                return BadRequest("Product ID is required");
            }

            if (productDto == null)
            {
                _logger.LogWarning("UpdateProduct called with null DTO");
                return BadRequest("Product data is required");
            }

            try
            {
                var product = new Product
                {
                    Id = id,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = productDto.Quantity,
                    CategoryId = productDto.CategoryId,
                    UpdatedAt = DateTime.UtcNow
                };

                if (!string.IsNullOrEmpty(productDto.ImageBase64))
                {
                    _logger.LogDebug("Processing product image update");
                    var base64Parts = productDto.ImageBase64.Split(',');
                    if (base64Parts.Length != 2)
                    {
                        _logger.LogWarning("Invalid image format during update");
                        return BadRequest("Invalid base64 image format.");
                    }

                    try
                    {
                        var contentTypePart = base64Parts[0].Split(':')[1].Split(';')[0];
                        var base64Data = base64Parts[1];
                        product.ImageContentType = contentTypePart;
                        product.ImageExtension = $".{contentTypePart.Split('/')[1]}";
                        product.ImageBytes = Convert.FromBase64String(base64Data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing product image during update");
                        return BadRequest("Invalid image data format");
                    }
                }

                await _productService.updateProduct(id, product);

                _logger.LogInformation("Product {ProductId} updated successfully", id);
                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Product Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product");
            }
        }

        [HttpGet("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products");

            try
            {
                var products = await _productService.GetAllProducts();

                _logger.LogInformation("Retrieved {ProductCount} products", products.Count());
                return Ok(new
                {
                    result = products,
                    status = StatusCodes.Status200OK,
                    message = "All Products Retrieved Successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving products");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(string id, [FromQuery] string categoryId)
        {
            _logger.LogInformation("Deleting product {ProductId}", id);
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("DeleteProduct called with empty ID");
                return BadRequest("Product ID is required");
            }
            try
            {
                await _productService.DeleteProduct(id);
                await _categoryService.UpdateProductCountByCategory(categoryId);
                _logger.LogInformation("Product {ProductId} deleted successfully", id);
                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Product Deleted Successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting product");
            }
        }

        [HttpGet("GetProductByid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        public async Task<IActionResult> GetProduct(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Product ID is required");
            }
            var product = await _productService.GetProductByid(id);
            return Ok(
                new
                {
                    data = product,
                    status = 200,
                    message = "Product Fetched Successfully"
                }
                );
        }
        [HttpGet]
        [Route("getProduuctByCAtegoryId/{id}")]
        public async Task<IActionResult> getProductByCategoryID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            else
            {
                var products = await _productService.GetProductsByCategoriesID(id);
                return Ok(new { result = products, status = 200 });
            }
        }
        [HttpGet]
        [Route("getproductDetails/{productId}")]
        public async Task<IActionResult> GetProductDetails(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest("Product ID cannot be null or empty");
            }

            if (!ObjectId.TryParse(productId, out _))
            {
                return BadRequest("Invalid Product ID format");
            }

            try
            {
                var productDocument = await _productService.GetProductDetailsAsync(productId);

                if (productDocument == null)
                {
                    return NotFound($"Product with ID {productId} not found");
                }
                var jsonSettings = new JsonWriterSettings
                {
                    Indent = true,
                };

                return Content(productDocument.ToJson(jsonSettings), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product details for {ProductId}", productId);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }


        [HttpGet]
        [Route("getsearchproducts/{name}")]
        public async Task<IActionResult> searchproduct(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { status = 400, message = "name is Empty" });
            }
            var result = await _productService.searchproducts(name);
            var jsonsettings = new JsonWriterSettings
            {
                Indent=true,
            };
            return Content(result.ToJson(jsonsettings), "application/json");
        }
    }
}