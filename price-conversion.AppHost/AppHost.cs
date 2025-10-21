using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);



var azureSql = builder.AddAzureSqlServer("sqldb").RunAsContainer();

var purchasedb = azureSql.AddDatabase("purchasedb");

var dbDeploy = builder.AddProject<Projects.purchasedb_databasedeploy>("db-deploy")
    .WithReference(purchasedb)
    .WaitFor(purchasedb);

var apiService = builder.AddProject<Projects.price_conversion_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var priceConversionDataApi = builder.AddProject<Projects.price_conversion_data_api>("price-conversion-api")
    .WaitFor(purchasedb)
    .WaitFor(dbDeploy)
    .WithReference(purchasedb)
    .WithHttpHealthCheck("/health");

var currencyConversionApi = builder.AddProject<Projects.currency_conversion_api>("currency-conversion-api");

builder.AddProject<Projects.price_conversion_web>("price-conversion-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(currencyConversionApi)
    .WaitFor(currencyConversionApi)
    .WithReference(priceConversionDataApi)
    .WaitFor(priceConversionDataApi);



builder.Build().Run();