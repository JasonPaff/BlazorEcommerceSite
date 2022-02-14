using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;
        private readonly ICartService _cartService;

        private const string secret = "whsec_156c702a7b9262b1a23c147d0cc218cf66f777eeab51a49bbd46d3fd3bf0025e";

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
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { "US" }
                },
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

        // fulfill customer order
        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
            // get json 
            var json = await new StreamReader(request.Body).ReadToEndAsync();

            try
            {
                // create stripe even with json + header + our secret
                var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], secret);

                // listen for checkout completed event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    // create session object from session
                    var session = stripeEvent.Data.Object as Session;
                    
                    // get user who ordered
                    var user = await _authService.GetUserByEmail(session.CustomerEmail);
                    
                    // place order
                    await _orderService.PlaceOrder(user.Id);
                }

                return new ServiceResponse<bool> {Data = true,};
            }
            catch (StripeException e)
            {
                return new ServiceResponse<bool> {Data = false, Success = false, Message = e.Message};
            }
        }
    }
}