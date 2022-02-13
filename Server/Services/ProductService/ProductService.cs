using System;
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
                .Include(p => p.Variants) // include Variants
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
                    .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower())) // matching category
                    .Include(p => p.Variants) // include Variants
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);
            List<string> result = new();

            foreach (var product in products)
            {
                // search product titles for words that contain matching text
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title); // add matching titles
                }

                // search product description for words that contain the text
                if (product.Description != null)
                {
                    // get the punctuation
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();

                    // get the individual words in the description
                    var words = product.Description.Split().Select(s => s.Trim(punctuation));

                    // find matching words
                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>> {Data = result};
        }

        // returns the featured products
        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Where(p => p.Featured)
                    .Include(p => p.Variants)
                    .ToListAsync()
            };

            return response;
        }

        // returns the products based on a text search
        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {
            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);
            var products = await _context.Products
                .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || p.Description
                .ToLower() // make everything lower case
                .Contains(searchText.ToLower())) // match text from title or description
                .Include(p => p.Variants) // include variants
                .Skip((page - 1) * (int) pageResults)
                .Take((int) pageResults)
                .ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>()
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };

            return response;
        }

        // finds the products based on a text search
        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || p.Description
                .ToLower() // make everything lower case
                .Contains(searchText.ToLower())) // match text from title or description
                .Include(p => p.Variants) // include variants
                .ToListAsync();
        }
    }
}