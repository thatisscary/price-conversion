namespace price_conversion_data_api_tests.Tests
{
    using Aspire.Hosting.Testing;
    using Microsoft.Extensions.Logging;
    using price_conversion_data_api.Controllers;
    using price_conversion_purchasedb.Entities;

    public class PurchaseControllerIntegrationTests
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(200);

        [Fact]
        public async Task GivenAValidPurchaseRequest_WhenCreating_ThenReturnsCreated()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.price_conversion_AppHost>(cancellationToken);
            appHost.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                // Override the logging filters from the app's configuration
                logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
                // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
            });
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            using var httpClient = app.CreateHttpClient("price-conversion-data-api");
            await app.ResourceNotifications.WaitForResourceHealthyAsync("price-conversion-data-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            // Act
            PurchaseRequest purchaseRequest = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase"
            };

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest), System.Text.Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync("api/purchases", content, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task GivenAValidPurchaseRequest_WhenLocationIsSet_ThenPurchaseCanBeRetrieved()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.price_conversion_AppHost>(cancellationToken);
            appHost.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                // Override the logging filters from the app's configuration
                logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
                // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
            });
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            using var httpClient = app.CreateHttpClient("price-conversion-data-api");
            await app.ResourceNotifications.WaitForResourceHealthyAsync("price-conversion-data-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            // Act
            PurchaseRequest purchaseRequest = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase"
            };

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest), System.Text.Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync("api/purchases", content, cancellationToken);

            HttpClient getClient = new HttpClient();

            var getResponse = await getClient.GetAsync(response.Headers?.Location?.ToString(), cancellationToken);

            var responseBody = await getResponse.Content.ReadAsStringAsync(cancellationToken);

            var purchaseResult = System.Text.Json.JsonSerializer.Deserialize<Purchase>(responseBody, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            var guidId = response.Headers?.Location?.Segments?.Last();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal("application/json; charset=utf-8", getResponse.Content.Headers.ContentType?.ToString());
            Assert.NotNull(purchaseResult);
            Assert.Equal(purchaseRequest.totalAmount, purchaseResult.TotalAmount);
            Assert.Equal(purchaseRequest.transactionDate.ToString("yyyy-MM-dd"), purchaseResult.TransactionDate.ToString("yyyy-MM-dd"));
            Assert.Equal(purchaseRequest.description, purchaseResult.Description);
            Assert.Equal(purchaseResult.PurchaseId.ToString(), guidId);
        }

        [Fact]
        public async Task GivenARequestForPurchases_WhenLocationIsSet_PurchaseCanBeRetrieved()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.price_conversion_AppHost>(cancellationToken);
            appHost.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                // Override the logging filters from the app's configuration
                logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
                // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
            });
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            using var httpClient = app.CreateHttpClient("price-conversion-data-api");
            await app.ResourceNotifications.WaitForResourceHealthyAsync("price-conversion-data-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            // Act
            PurchaseRequest purchaseRequest1 = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase-1"
            };

            PurchaseRequest purchaseRequest2 = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase-2"
            };

            HttpContent content1 = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest1), System.Text.Encoding.UTF8, "application/json");

            HttpContent content2 = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest2), System.Text.Encoding.UTF8, "application/json");

            await httpClient.PostAsync("api/purchases", content1, cancellationToken);

            await httpClient.PostAsync("api/purchases", content2, cancellationToken);

            await httpClient.GetAsync("api/purchases", cancellationToken);

            var getResponse = await httpClient.GetAsync("api/purchases", cancellationToken);

            var responseBody = await getResponse.Content.ReadAsStringAsync(cancellationToken);

            var purcahseList = System.Text.Json.JsonSerializer.Deserialize<List<Purchase>>(responseBody, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal("application/json; charset=utf-8", getResponse.Content.Headers.ContentType?.ToString());
            Assert.NotNull(purcahseList);
            Assert.True(purcahseList.Count >= 2);
            Assert.Contains(purcahseList, p => p.Description == purchaseRequest1.description);
            Assert.Contains(purcahseList, p => p.Description == purchaseRequest2.description);
        }
    }
}