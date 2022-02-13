using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        // inject product service
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        // get all products
        [HttpGet] 
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            var result = await _productService.GetProductsAsync();
            return Ok(result);
        }

        // get specific product
        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
        {
            var result = await _productService.GetProductAsync(productId);
            return Ok(result);
        }
        
        // get products based on a search term 
        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(string searchText)
        {
            var result = await _productService.SearchProducts(searchText);
            return Ok(result);
        }
        
        // get specific product 
        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryUrl);
            return Ok(result);
        }
    }
}