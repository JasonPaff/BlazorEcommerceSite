using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        List<Product> Products { get; set; }
        Task GetProducts();
        Task<ServiceResponse<Product>?> GetProduct(int productId);
    }
}