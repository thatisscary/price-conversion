namespace price_conversion_data_api_tests.Tests
{
    using Microsoft.Extensions.Logging;

    public class IntegrationTest1
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);


        [Fact]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.price_conversion_AppHost> (cancellationToken);
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

            // Act
            using var httpClient = app.CreateHttpClient("price-conversion-data-api");
            await app.ResourceNotifications.WaitForResourceHealthyAsync("price-conversion-data-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            using var response = await httpClient.GetAsync("/", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
