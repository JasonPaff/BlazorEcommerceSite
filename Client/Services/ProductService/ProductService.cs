using System;
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

        public List<Product> Products { get; set; } = new();
        public string Message { get; set; } = "Loading Products...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; }
        public string LastSearchText { get; set; } = string.Empty;

        public event Action ProductsChanged;

        // get all products
        public async Task GetProducts(string? categoryUrl = null)
        {
            // get all products or category specific products
            var result = categoryUrl == null
                ? await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured")
                : await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
            if (result is {Data: { }})
                Products = result.Data;

            CurrentPage = 1;
            PageCount = 0;

            if (Products.Count is 0)
                Message = "No products found";
            
            // fire products changed event
            ProductsChanged.Invoke();
        }

        // get a single product from the products controller on the server
        public async Task<ServiceResponse<Product>?> GetProduct(int productId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;
        }

        // search for products matching search text
        public async Task SearchProducts(string searchText, int page)
        {
            LastSearchText = searchText;
            var result =
                await _http.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"api/product/search/{searchText}/{page}");

            if (result is {Data: { }})
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }

            if (Products.Count == 0) Message = "No products found.";

            ProductsChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result =
                await _http.GetFromJsonAsync<ServiceResponse<List<string>>>(
                    $"api/product/searchSuggestions/{searchText}");

            return result.Data;
        }
    }
}