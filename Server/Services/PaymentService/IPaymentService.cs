using System.Threading.Tasks;
using Stripe.Checkout;

namespace ECommerce.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Session> CreateCheckoutSession();
    }
}