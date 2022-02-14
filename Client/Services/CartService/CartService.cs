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
        public CartService(ILocalStorageService localStorage, HttpClient http,
            AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public event Action? OnChange;

        // add items to cart
        public async Task AddToCart(CartItem cartItem)
        {
            // user is authenticated
            if (await IsUserAuthenticated())
            {
                // get cart from database
                await _http.PostAsJsonAsync("api/cart/add", cartItem);

                // update cart items count
                await GetCartItemsCount();

                return;
            }
            // user is not authenticated

            // get cart from local storage, create a new one if no cart is found
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart") ?? new List<CartItem>();

            // check for item in cart already
            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

            // add item to cart or increase quantity of existing item
            if (sameItem is null) cart.Add(cartItem);
            else sameItem.Quantity += cartItem.Quantity;

            // update local storage with new cart
            await _localStorage.SetItemAsync("cart", cart);

            // update cart items count
            await GetCartItemsCount();
        }

        // remove an item from the cart
        public async Task RemoveFromCart(int productId, int productTypeId)
        {
            // user is authenticated
            if (await IsUserAuthenticated())
            {
                // get cart from database
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");

                return;
            }
            // user is not authenticated
            
            // get the cart items
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            // no items to remove
            if (cart is null || cart.Count is 0) return;

            // find cart item
            var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

            // no cart item found
            if (cartItem is null) return;

            // remove item
            cart.Remove(cartItem);

            // update cart
            await _localStorage.SetItemAsync("cart", cart);
        }

        // return all products based on cart items
        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if (await IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                // get cart items from local storage
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cartItems is null) return new List<CartProductResponse>();

                // http post request and response
                var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);

                // get cart products from response
                var cartProducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartProducts.Data;
            }
        }

        // update cart item quantity
        public async Task UpdateQuantity(CartProductResponse product)
        {
            if (await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _http.PutAsJsonAsync("api/cart/update-quantity", request);

                return;
            }
            
            // get the cart items
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            // no items to remove
            if (cart is null || cart.Count is 0) return;

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
                //OnChange?.Invoke();
            }
        }

        // save cart to database
        public async Task StoreCartItems(bool emptyLocalCart = false)
        {
            // get cart from local storage
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            // no cart, return
            if (localCart is null) return;

            // save cart
            await _http.PostAsJsonAsync("api/cart", localCart);

            // empty cart if requested
            if (emptyLocalCart)
                await _localStorage.RemoveItemAsync("cart");
        }

        // return number of items in users cart
        public async Task GetCartItemsCount()
        {
            if (await IsUserAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                await _localStorage.SetItemAsync<int>("cartItemsCount", result.Data);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount", cart?.Count ?? 0);
            }
            
            // notify ui of change
            OnChange?.Invoke();
        }

        // true/false if user is authenticated
        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}