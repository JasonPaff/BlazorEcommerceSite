﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // inject database context, http context
        public CartService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // get user id from http context
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        
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
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity
                };
                
                // add product to results
                result.Data.Add(cartProduct);
            }

            return result;
        }

        // store users cart in database
        public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
        {
            // set user id
            cartItems.ForEach(cartItem => cartItem.UserId = GetUserId());
            
            // add items
            _context.CartItems.AddRange(cartItems);
            
            // save cart
            await _context.SaveChangesAsync();

            // return cart items
            return await GetDbCartProducts();
        }

        // return number of items in user cart
        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = (await _context.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };
        }

        // return cart items from database for authenticated user
        public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
        {
            return await GetCartProducts(await _context.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync());
        }

        // add item to cart in database
        public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
        {
            // set user id
            cartItem.UserId = GetUserId();
            
            // look for item in database
            var sameItem = await _context.CartItems.FirstOrDefaultAsync(ci =>
                ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == cartItem.UserId);

            // add item or update quantity depending
            if (sameItem is null) _context.CartItems.Add(cartItem);
            else sameItem.Quantity += cartItem.Quantity;

            // save changes to database
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> {Data = true};
        }

        // update cart quantity in database
        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
        {            
            // set user id
            cartItem.UserId = GetUserId();
            
            // look for item in database
            var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci =>
                ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == cartItem.UserId);

            // no item, failure
            if (dbCartItem is null) return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };

            // update cart item quantity
            dbCartItem.Quantity = cartItem.Quantity;
            
            // save changes to database
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> {Data = true};
        }
    }
}