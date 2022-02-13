﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;

        // inject database context
        public CartService(DataContext context)
        {
            _context = context;
        }
        
        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
        {
            // hold our result
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>()
            };

            // loop cart items to build products
            foreach (var item in cartItems)
            {
                // get matching product
                var product = await _context.Products.Where(p => p.Id == item.ProductId).FirstOrDefaultAsync();

                if (product is null) continue;

                // get variant
                var productVariant = await _context.ProductVariants.Where(v =>
                        v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant is null) continue;

                // create product
                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId
                };
                
                // add product to results
                result.Data.Add(cartProduct);
            }

            return result;
        }
    }
}