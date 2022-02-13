using System.Collections.Generic;
using System.Linq;
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

        // return all products
        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Include(p => p.Variants) // include Variants
                    .ToListAsync()
            };

            return response;
        }

        // return a single product
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            var product = await _context.Products
                .Include(p => p.Variants)// include Variants
                .ThenInclude(v => v.ProductType) // include ProductTypes
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                response.Success = false;
                response.Message = "Sorry, but the product does not exist.";
            }
            else
                response.Data = product;

            return response;
        }

        // return the products based on a category
        public async Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))// matching category
                    .Include(p => p.Variants) // include Variants
                    .ToListAsync()
            };

            return response;
        }
    }
}