using System.Threading.Tasks;

namespace ECommerce.Client.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegister request);
    }
}