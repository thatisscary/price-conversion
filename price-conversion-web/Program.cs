using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using price_conversion_web.Contracts;
using price_conversion_web.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSerilog();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.WriteIndented = false;
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Default;
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    options.JsonSerializerOptions.MaxDepth = 3;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddHttpClient<CurrencyConversionService>(
    httpsClient =>
    {
        httpsClient.BaseAddress = new Uri("https://currency-conversion-api"); // Replace with your actual base URL
        httpsClient.DefaultRequestHeaders.Add("Accept", "application/json");
        // Add any other configurations, like timeouts or headers
    });
 
builder.Services.AddHttpClient<PurchaseDataService>(
    httpsClient =>
    {
        httpsClient.BaseAddress = new Uri("https://price-conversion-data-api"); // Replace with your actual base URL
        httpsClient.DefaultRequestHeaders.Add("Accept", "application/json");
        // Add any other configurations, like timeouts or headers
    });


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
