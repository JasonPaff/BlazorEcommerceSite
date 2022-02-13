using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        // inject http client
        public AuthService(HttpClient http)
        {
            _http = http;
        }
        
        // register with auth controller
        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            // send request to auth controller
            var result = await _http.PostAsJsonAsync("api/auth/register", request);
            
            // return response from auth controller
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }

        // login with auth controller
        public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            // send request to auth controller
            var result = await _http.PostAsJsonAsync("api/auth/login", request);
            
            // return response from auth controller
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }
    }
}