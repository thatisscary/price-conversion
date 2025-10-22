namespace currency_conversion_api_tests.Tests
{
    using currency_conversion_api.Contracts;
    using Microsoft.Extensions.Logging;

    public class CurrencyConversionApiIntegrationTests
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(200);

        [Fact]
        public async Task GivenARequestForAvailableCurrencies_ThenCurrenciesAreReturned()
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

            // Act
            using var httpClient = app.CreateHttpClient("currency-conversion-api");

            await app.ResourceNotifications.WaitForResourceHealthyAsync("currency-conversion-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
            using var response = await httpClient.GetAsync("api/currencies", cancellationToken);

            AvailableCurrencies? currencies = null;
            using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                using var sr = new StreamReader(responseStream);
                var content = await sr.ReadToEndAsync();
                currencies = System.Text.Json.JsonSerializer.Deserialize<AvailableCurrencies>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.NotNull(currencies);
            Assert.NotEmpty(currencies.Currencies);
            Assert.Contains(currencies.Currencies, c => c.CurrencyIdentifier == "Japan-Yen");
        }

        [Fact]
        public async Task GivenARequestForExchangeRate_WhenValidDateIsGiven_ExchangeRateIsReturnedd()
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

            // Act
            using var httpClient = app.CreateHttpClient("currency-conversion-api");

            await app.ResourceNotifications.WaitForResourceHealthyAsync("currency-conversion-api", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

            var currencyCode = "Japan-Yen";
            var transactionDate = DateTime.Now; 
            using var response = await httpClient.GetAsync($"api/currencies/exchangerate/{currencyCode}/{transactionDate:yyyy-MM-dd}", cancellationToken);

            ExchangeRateResponse? currencies = null;
            using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                using var sr = new StreamReader(responseStream);
                var content = await sr.ReadToEndAsync();
                currencies = System.Text.Json.JsonSerializer.Deserialize<ExchangeRateResponse>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.NotNull(currencies);
            Assert.Equal(currencies.CurrencyIdentifier, currencyCode);
            Assert.True(currencies.ExchangeRate > 0);

        }
    }
}