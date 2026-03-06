using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceDemo.Application.DTOs.Product;
using ServiceDemo.Application.Services.Interfaces;
using ServiceDemo.Application.Validators.Product;

namespace ServiceDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly CreateProductValidator _createProductValidator;
        private readonly UpdateProductValidator _updateProductValidator;

        public ProductController( IProductService productService, CreateProductValidator createProductValidator,
         UpdateProductValidator updateProductValidator, ILogger<ProductController> logger)
        {
            _logger = logger;
            _productService = productService;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="createProductDto">Product data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var validationResult = await _createProductValidator.ValidateAsync(createProductDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _productService.CreateProductAsync(createProductDto);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = result.Data!.Id }, result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Gets an product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Gets all products with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            if (page < 0) page = 0;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _productService.GetProductsAsync(page, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Updated product data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto updateProductDto)
        {
            var validationResult = await _updateProductValidator.ValidateAsync(updateProductDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _productService.UpdateProductAsync(id, updateProductDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

         /// <summary>
        /// Deletes a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}