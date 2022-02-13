using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        // inject local storage service, http client, auth state provider
        public CartService(ILocalStorageService localStorage, HttpClient http, AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public event Action? OnChange;

        // add items to cart
        public async Task AddToCart(CartItem cartItem)
        {
            if (_authStateProvider.GetAuthenticationStateAsync().User.Identity.IsAuthenticated)
            {
                Console.WriteLine("yes authed");
            }
            else
            {
                Console.WriteLine("no authed");
            }
            
            // get cart from local storage, create a new one if no cart is found
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart") ?? new List<CartItem>();

            // check for item in cart already
            var sameItem =
                cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
            
            // add item to cart or increase quantity of existing item
            if (sameItem is null) cart.Add(cartItem);
            else sameItem.Quantity += cartItem.Quantity;

            // update local storage with new cart
            await _localStorage.SetItemAsync("cart", cart);

            // notify app of cart count change
            OnChange?.Invoke();
        }

        // remove an item from the cart
        public async Task RemoveFromCart(int productId, int productTypeId)
        {
            // get the cart items
            var cart = await GetCartItems();

            // no items to remove
            if (cart.Count is 0) return;

            // find cart item
            var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

            // remove cart item
            if (cartItem is not null)
            {
                // remove item
                cart.Remove(cartItem);

                // update cart
                await _localStorage.SetItemAsync("cart", cart);
                
                // notify of update
                OnChange?.Invoke();
            }
        }

        // return all items in the cart
        public async Task<List<CartItem>> GetCartItems()
        {
            // get cart from local storage, create a new one if no cart is found
            return await _localStorage.GetItemAsync<List<CartItem>>("cart") ?? new List<CartItem>();
        }

        // return all products based on cart items
        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            // get cart items from local storage
            var cartItems = await GetCartItems();

            // http post request and response
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);

            // get cart products from response
            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartProducts.Data;
        }

        // update cart item quantity
        public async Task UpdateQuantity(CartProductResponse product)
        {
            // get the cart items
            var cart = await GetCartItems();

            // no items to remove
            if (cart.Count is 0) return;

            // find cart item
            var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

            // update cart item quantity
            if (cartItem is not null)
            {
                // update quantity
                cartItem.Quantity = product.Quantity;
                
                // update cart
                await _localStorage.SetItemAsync("cart", cart);
                
                // notify of update
                OnChange?.Invoke();
            }
        }
    }
}