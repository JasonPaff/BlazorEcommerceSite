using System.Threading.Tasks;

namespace ECommerce.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task PlaceOrder();
    }
}