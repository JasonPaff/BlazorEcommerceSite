using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;

        // inject http service
        public ProductService(HttpClient http)
        {
            _http = http; 
        }
        
        // products list
        public List<Product> Products { get; set; } = new();
        
        // get all products from the products
        public async Task GetProducts()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
            if (result is {Data: { } }) 
                Products = result.Data;
        }

        // get a single product from the products controller on the server
        public async Task<ServiceResponse<Product>?> GetProduct(int productId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;
        }
    }
}