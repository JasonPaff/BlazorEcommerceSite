using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        // database context
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context; // inject database context
        }

        // return the products from the database
        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.ToListAsync()
            };

            return response;
        }

        // return a single product from the database
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            var product = await _context.Products.FindAsync(productId);

            if (product is null)
            {
                response.Success = false;
                response.Message = "Sorry, but the product does not exist.";
            }
            else
                response.Data = product;

            return response;
        }
    }
}