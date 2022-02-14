using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;

namespace ECommerce.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Session> CreateCheckoutSession();
        Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request);
    }
}