using Google.Protobuf.WellKnownTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using price_conversion_purchasedb;
using price_conversion_purchasedb_migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Query;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<PurchaseDbContext>("purchasedb", null,
    optionsBuilder => optionsBuilder.UseSqlServer(sqlBuilder =>
        sqlBuilder.MigrationsAssembly(typeof(Program).Assembly.GetName().Name) ));

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(PurchaseDbInitializer.ActivitySourceName));

builder.Services.AddSingleton<PurchaseDbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PurchaseDbInitializer>());
builder.Services.AddHealthChecks()
    .AddCheck<PurchaseDbMigrationHealthCheck>("DbInitializer", null);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/reset-db", async (PurchaseDbContext dbContext, PurchaseDbInitializer dbInitializer, CancellationToken cancellationToken) =>
    {
        // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbInitializer.InitializeDatabaseAsync(dbContext, cancellationToken);
    });
}

app.MapDefaultEndpoints();

await app.RunAsync();