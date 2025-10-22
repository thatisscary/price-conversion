namespace price_conversion_currency_api_testing
{
    using System.Threading;
    using currency_conversion_api.Contracts;

    public class CurrencyControllerTests
    {
        [ClassDataSource<Data.HttpClientDataClass>]
        [Test]
        public async Task GivenARequestForAvailableCurrencies_ThenCurrenciesAreReturned(Data.HttpClientDataClass httpClientData)
        {
            // Arrange
            var httpClient = httpClientData.HttpClient;

            var cancellationToken = CancellationToken.None;

            // Act
            var response = await httpClient.GetAsync("api/currencies", cancellationToken);

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
            Assert.Equals(HttpStatusCode.OK, response.StatusCode);
            Assert.Equals("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            await Assert.That(currencies).IsNotNull();
            await Assert.That(currencies?.Currencies).IsNotEmpty();
            await Assert.That(currencies?.Currencies).Contains(c => c.CurrencyIdentifier == "Japan-Yen");
        }

        [ClassDataSource<Data.HttpClientDataClass>]
        [Test]
        public async Task GivenARequestForExchangeRate_WhenValidDateIsGiven_ExchangeRateIsReturnedd(Data.HttpClientDataClass httpClientData)
        {
            // Arrange
            HttpClient httpClient = httpClientData.HttpClient;
            var currencyCode = "Japan-Yen";
            var transactionDate = DateTime.Now;
            
            var cancellationToken = CancellationToken.None;

            // Act
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
            Assert.Equals(HttpStatusCode.OK, response.StatusCode);
            Assert.Equals("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            await Assert.That(currencies).IsNotNull();
            Assert.Equals(currencies?.CurrencyIdentifier, currencyCode);
            await Assert.That(currencies?.ExchangeRate).IsNotNull().And.IsGreaterThanOrEqualTo(0);


        }
    }
}