using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        event Action ProductsChanged;
        List<Product> Products { get; set; }
        Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>?> GetProduct(int productId);
    }
}