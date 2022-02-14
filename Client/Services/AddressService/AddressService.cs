using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _http;

        // Inject http client.
        public AddressService(HttpClient http)
        {
            _http = http;
        }
        
        // Get user shipping address.
        public async Task<Address> GetAddress()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<Address>>("api/address");
            return response.Data;
        }

        // Add or update user shipping address.
        public async Task<Address> AddOrUpdateAddress(Address address)
        {
            var response = await _http.PostAsJsonAsync("api/address", address);
            return response.Content.ReadFromJsonAsync<ServiceResponse<Address>>().Result.Data;
        }
    }
}