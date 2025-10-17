using currency_conversion_api.Services;
using FluentValidation;
using MediatR;
using price_conversion_common.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddMediatR(cfg =>
{
     cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkyMTA4ODAwIiwiaWF0IjoiMTc2MDY0OTgxNSIsImFjY291bnRfaWQiOiIwMTk5ZWVlNzQzZDA3NGU0YTJmNGJlMTFiMzk5NzNlOSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazdxZWc5d3M1dzBybXoyZGVqcm02ZnZiIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.Kn7xE7W8cn4eKkttTVfgcOas9gRCZ3JFq7RgwlZsUIebfr6tTad55xpZpEj2p1cmlSl3Qb05srfJ0d8SysgqxbKXIqbJkWSHcqUKhyxAfUX-tTnan3gUeL6FMPh8eYEbg4zVZnDbHy-u0RRlYr9jscakumOxBS2UQ_uixT9DAmgJhHuslQofRV2DQGgedrvLPY_YB5QffquYQaq7UtaHnhmHFUZb28nDvBNFx3aKvT8eKIiIch4vBBFwe0hXD0TzRWSRUUzy1bB8h_JVRoANELVjCBCKm2Yiass7y6lXkVtveXPVLhVECE4sLhiDDXhEorKjeKXlDWxgZjjD8mPXfg";
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Register custom pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));

builder.Services.AddTransient<IForeignCurrencyService, ForeignCurrencyService>();

// Register all Fluent Validators

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

builder.Services.AddHttpClient<ForeignCurrencyService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStatusCodePages();

app.Run();