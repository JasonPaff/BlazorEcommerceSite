global using ECommerce.Shared;
global using ECommerce.Client.Services.ProductService;
global using ECommerce.Client.Services.CategoryService;
global using ECommerce.Client.Services.AuthService;
using System;
using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ECommerce.Client;
using ECommerce.Client.Services.CartService;
using Microsoft.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// local storage
builder.Services.AddBlazoredLocalStorage();

// http client service
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// product service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
