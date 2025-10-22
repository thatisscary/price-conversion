using currency_conversion_api.Services;
using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;
using LiteBus.Queries.Extensions.MicrosoftDependencyInjection;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSerilog();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddLiteBus(liteBus =>
{
    liteBus.AddCommandModule(module =>
    {
        module.RegisterFromAssembly(typeof(Program).Assembly);
    });
    liteBus.AddQueryModule(module =>
    {
        module.RegisterFromAssembly(typeof(Program).Assembly);
    });
});


builder.Services.AddHttpClient<ForeignCurrencyService>(client =>
{
    client.BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange/"); // Replace with your actual base URL
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    // Add any other configurations, like timeouts or headers
});



var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
