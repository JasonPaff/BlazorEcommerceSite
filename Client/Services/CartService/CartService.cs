﻿using System;
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
            // check authentication
            if (!await IsUserAuthenticated()) return;

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

            // update cart items count
            await GetCartItemsCount();
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

            // no cart item found
            if (cartItem is null) return;

            // remove item
            cart.Remove(cartItem);

            // update cart
            await _localStorage.SetItemAsync("cart", cart);

            // update cart item count
            await GetCartItemsCount();
        }

        // return all items in the cart
        public async Task<List<CartItem>> GetCartItems()
        {
            // update cart items count
            await GetCartItemsCount();

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
            if (cartItem is null) return;

            // update quantity
            cartItem.Quantity = product.Quantity;

            // update cart
            await _localStorage.SetItemAsync("cart", cart);

            // update cart items count
            await GetCartItemsCount();
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

            OnChange?.Invoke();
        }

        // true/false if user is authenticated
        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}