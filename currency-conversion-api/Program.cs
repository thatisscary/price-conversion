using currency_conversion_api.Services;
using MediatR;
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

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkyMTA4ODAwIiwiaWF0IjoiMTc2MDY0OTgxNSIsImFjY291bnRfaWQiOiIwMTk5ZWVlNzQzZDA3NGU0YTJmNGJlMTFiMzk5NzNlOSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazdxZWc5d3M1dzBybXoyZGVqcm02ZnZiIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.Kn7xE7W8cn4eKkttTVfgcOas9gRCZ3JFq7RgwlZsUIebfr6tTad55xpZpEj2p1cmlSl3Qb05srfJ0d8SysgqxbKXIqbJkWSHcqUKhyxAfUX-tTnan3gUeL6FMPh8eYEbg4zVZnDbHy-u0RRlYr9jscakumOxBS2UQ_uixT9DAmgJhHuslQofRV2DQGgedrvLPY_YB5QffquYQaq7UtaHnhmHFUZb28nDvBNFx3aKvT8eKIiIch4vBBFwe0hXD0TzRWSRUUzy1bB8h_JVRoANELVjCBCKm2Yiass7y6lXkVtveXPVLhVECE4sLhiDDXhEorKjeKXlDWxgZjjD8mPXfg";
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});



builder.Services.AddHttpClient<ForeignCurrencyService>(client =>
{
    client.BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange/"); // Replace with your actual base URL
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    // Add any other configurations, like timeouts or headers
});


// Register custom pipeline behaviors
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));



// Register all Fluent Validators

//builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));



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
