using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace ECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;

        // inject local storage service
        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        
        public event Action? OnChange;
        
        // add items to cart
        public async Task AddToCart(CartItem cartItem)
        {
            // get cart from local storage, create a new one if no cart is found
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart") ?? new List<CartItem>();
            
            // add item to cart
            cart.Add(cartItem);

            // update local storage with new cart
            await _localStorage.SetItemAsync("cart", cart);
        }

        // return all items in the cart
        public async Task<List<CartItem>> GetCartItems()
        {
            // get cart from local storage, create a new one if no cart is found
            return await _localStorage.GetItemAsync<List<CartItem>>("cart") ?? new List<CartItem>();
        }
    }
}