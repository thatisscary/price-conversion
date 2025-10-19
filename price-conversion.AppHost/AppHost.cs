using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var azureSql = builder.AddAzureSqlServer("sqldb").RunAsContainer();

var purchasedb = azureSql.AddDatabase("purchasedb");

var dbDeploy = builder.AddProject<Projects.purchasedb_databasedeploy>("dbDeploy")
    .WithReference(purchasedb)
    .WaitFor(purchasedb);

var apiService = builder.AddProject<Projects.price_conversion_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.price_conversion_data_api>("price-conversion-data-api")
    .WaitFor(purchasedb)
    .WaitFor(dbDeploy)
    .WithReference(purchasedb)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.currency_conversion_api>("currency-conversion-api");

builder.AddProject<Projects.price_conversion_web>("price-conversion-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();