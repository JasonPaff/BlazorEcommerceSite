using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ECommerce.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        // inject local storage service, http client
        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // get jwt from local storage
            string authToken = await _localStorage.GetItemAsStringAsync("authToken");

            // new unauthorized identity
            var identity = new ClaimsIdentity();
            
            // unauthorized
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    // set identity
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    
                    // set jwt, remove quotes
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"",""));
                }
                catch
                {
                    // remove token
                    await _localStorage.RemoveItemAsync("authToken");
                    
                    // new unauthorized identity
                    identity = new ClaimsIdentity();
                }
            }

            // user claim
            var user = new ClaimsPrincipal(identity);
            
            // user auth state
            var state = new AuthenticationState(user);
            
            // notify auth state changed
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        // convert string to bytes
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        // create claims from jwt
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            // split jwt
            var payload = jwt.Split('.')[1];
            
            // convert to bytes
            var jsonBytes = ParseBase64WithoutPadding(payload);
            
            // create key/value pairs
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // create claims from key/value pairs
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

            return claims;
        }
    }
}