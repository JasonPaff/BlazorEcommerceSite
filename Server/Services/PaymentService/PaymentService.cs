using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;
        private readonly ICartService _cartService;

        // inject order service, auth service, cart service
        public PaymentService(IOrderService orderService, IAuthService authService, ICartService cartService)
        {
            StripeConfiguration.ApiKey =
                "sk_test_51KRQvQHL4ylhr8vWV0iiirvTqtsqOqpZ98U4uc9XBDx4kpCbGYUHdewJCyUUwfYylrjAX5PUBHxsU9f9A58Saka200yO9DutSZ";
            
            _orderService = orderService;
            _authService = authService;
            _cartService = cartService;
        }
        
        public async Task<Session> CreateCheckoutSession()
        {
            // get products to buy
            var products = (await _cartService.GetDbCartProducts()).Data;
            
            var lineItems = new List<SessionLineItemOptions>();
            
            // create line items from the products
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Title,
                        Images = new List<string> {product.ImageUrl}
                    }
                },
                Quantity = product.Quantity
            }));

            // create checkout options
            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7109/order-success",
                CancelUrl = "https://localhost:7109/cart"
            };

            var service = new SessionService();
            
            // create checkout session
            Session session = service.Create(options);
            
            return session;
        }
    }
}