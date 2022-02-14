using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Server.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        // inject order service
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // add order to database
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<bool>>> PlaceOrder()
        {
            var result = await _orderService.PlaceOrder();
            return Ok(result);
        }
    }
}