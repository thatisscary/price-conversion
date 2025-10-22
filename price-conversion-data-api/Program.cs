using Serilog;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;
using LiteBus.Queries.Extensions.MicrosoftDependencyInjection;
using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using price_conversion_data_api.Repositories;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("Starting up Price Conversion Data API");

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddSerilog();

builder.AddSqlServerDbContext<price_conversion_purchasedb.PurchaseDbContext>("purchasedb");

builder.Services.AddTransient<IPurchaseRepository, PurchaseRepository>();

builder.Services.AddLiteBus(liteBus =>
{
    liteBus.AddQueryModule(module =>
    {
        module.RegisterFromAssembly(typeof(Program).Assembly);
    });

    liteBus.AddCommandModule(module =>
    {
        module.RegisterFromAssembly(typeof(Program).Assembly);
    });
});



builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
var variables = Environment.GetEnvironmentVariables();
var connectionstrings = variables["ConnectionStrings__purchasedb"];
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
