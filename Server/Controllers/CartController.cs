using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        // inject cart service
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // get cart items
        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(
            List<CartItem> cartItems)
        {
            var result = await _cartService.GetCartProducts(cartItems);
            return Ok(result);
        }

        // get cart items
        [HttpPost()]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(
            List<CartItem> cartItems)
        {
            var result = await _cartService.StoreCartItems(cartItems);
            return Ok(result);
        }
        
        // get cart items count
        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()

        {
            return await _cartService.GetCartItemsCount();
        }
    }
}