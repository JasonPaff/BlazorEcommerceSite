using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // inject database context, cart service, http context service
        public OrderService(DataContext context, ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        // get user id from http context
        private int GetUserId() =>
            int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        // place an order
        public async Task<ServiceResponse<bool>> PlaceOrder()
        {
            // get database cart products
            var products = (await _cartService.GetDbCartProducts()).Data;

            decimal totalPrice = 0;
            
            // add up total price
            products.ForEach(product => totalPrice += product.Price * product.Quantity);

            var orderItems = new List<OrderItem>();

            // create order items from products
            products.ForEach(product => orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));

            // create order
            var order = new Order
            {
                UserId = GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            // add order to database
            _context.Orders.Add(order);

            // save changes to database
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> {Data = true};
        }
    }
}