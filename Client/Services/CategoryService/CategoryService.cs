using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _http;

        // inject http service
        public CategoryService(HttpClient http)
        {
            _http = http;
        }

        // category list
        public List<Category> Categories { get; set; }
        
        // get all categories from category controller
        public async Task GetCategories()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
            if (response is {Data: {} })
                Categories = response.Data;
        }
    }
}