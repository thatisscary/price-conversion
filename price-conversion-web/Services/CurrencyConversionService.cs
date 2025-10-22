namespace price_conversion_web.Services
{
    using System.Text.Json;
    using price_conversion_web.Contracts;

    public class CurrencyConversionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyConversionService> _logger;

        public CurrencyConversionService(HttpClient httpClient, ILogger<CurrencyConversionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CurrencyExchange?> GetConversionRateAsync(string currencyCode, DateTime transactionDate)
        {
            try
            {
                var requestUri = $"{_httpClient.BaseAddress}api/currencies/exchangerate/{currencyCode}/{transactionDate:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    using var sr = await response.Content.ReadAsStreamAsync();
                    JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var exchangeRate = await JsonSerializer.DeserializeAsync<CurrencyExchange>(sr, options);
                    exchangeRate!.ConversionFound = true;
                    return exchangeRate;
                }

                if( response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new CurrencyExchange { ConversionFound = false , ErrorMessage = $"No conversion found for {currencyCode} for transaction date {transactionDate:yyyy-MM-dd}" };
                    _logger.LogWarning("Failed to get conversion rate. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching conversion rate for {Currency} on {Date}", currencyCode, transactionDate);
                return new CurrencyExchange { ConversionFound = false, ErrorMessage = $"Error Retrieving Conversion Rate found for {currencyCode} for transaction date {transactionDate:yyyy-MM-dd}" };
            }
            return new CurrencyExchange { ConversionFound = false }; 
        }

        public async Task<AvailableCurrencies> GetForeignCurrencies()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/currencies");
                if (response.IsSuccessStatusCode)
                {
                    using (var sr = await response.Content.ReadAsStreamAsync())
                    {
                        if (sr != null)
                        {
                            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var availableCurrencies = await JsonSerializer.DeserializeAsync<AvailableCurrencies>(sr, options);

                            return availableCurrencies;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to get supported currencies. Status Code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching supported currencies.");
            }

            return null;
        }

        //api/currencies
    }
}