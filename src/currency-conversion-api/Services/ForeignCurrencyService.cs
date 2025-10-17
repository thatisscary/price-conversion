namespace currency_conversion_api.Services
{
    using System.IO;
    using System.Net;
    using System.Text.Json;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Contracts.External;
    using Microsoft.AspNetCore.Mvc;
    using OneOf;

    public class ForeignCurrencyService
    {
        private readonly HttpClient _client;

        private const string AllCurrencyDescriptionsQuery = "?page[size]=1000&sort=country_currency_desc&fields=country_currency_desc";

        public ForeignCurrencyService(HttpClient client, ILogger<ForeignCurrencyService> _logger )
        {
            _client = client;
            _logger.LogInformation("ForeignCurrencyService initialized with base address: {BaseAddress}", _client.BaseAddress);
        }

        public async Task<FiscalData<ForeignCurrencyDescription>> GetAvailableCurrencies()
        {
            using (var result = await _client.GetAsync(_client.BaseAddress + AllCurrencyDescriptionsQuery))
            {
                if (result.IsSuccessStatusCode)
                {
                    using var currencyListStream = await result.Content.ReadAsStreamAsync();
                    if (currencyListStream != null)
                    {
                        using (var sr = await result.Content.ReadAsStreamAsync())
                        {
                            if (sr != null)
                            {
                                FiscalData<ForeignCurrencyDescription> currencyList = await JsonSerializer.DeserializeAsync<FiscalData<ForeignCurrencyDescription>>(sr);

                                return currencyList;
                            }
                        }
                    }
                }

                return new FiscalData<ForeignCurrencyDescription>();
            }
        }

        public async Task<FiscalData<ExchangeRateItem>> GetConversionRate(CurrencyConversionRateRequest request)
        {
            var exchangeRateQueryString = $"{_client.BaseAddress}?page[size]=4&sort=-effective_date&fields=effective_date,exchange_rate,country_currency_desc,record_date&filter=country_currency_desc:eq:{request.CurrencyIdentifier},effective_date:lte:{request.TransactionDate:yyyy-MM-dd}";

            using (var result = await _client.GetAsync(exchangeRateQueryString))
            {
                if (result.IsSuccessStatusCode)
                {
                    using (var sr = await result.Content.ReadAsStreamAsync())
                    {
                        if (sr != null)
                        {
                            FiscalData<ExchangeRateItem> exchangeRateData = JsonSerializer.Deserialize<FiscalData<ExchangeRateItem>>(sr);

                            return exchangeRateData;
                        }
                    }
                }

                return new FiscalData<ExchangeRateItem>();
            }
        }
    }
}