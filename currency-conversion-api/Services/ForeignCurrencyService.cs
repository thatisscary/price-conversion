namespace currency_conversion_api.Services
{
    using System.IO;
    using System.Net;
    using System.Text.Json;
    using currency_conversion_api.Contracts;
    using currency_conversion_api.Contracts.External;
    using Microsoft.AspNetCore.Mvc;

    public class ForeignCurrencyService
    {
        private readonly HttpClient _client;

        private const string AllCurrencyDescriptionsQuery = "?page[size]=1000&sort=country_currency_desc&fields=country_currency_desc";

        public ForeignCurrencyService(HttpClient client, ILogger<ForeignCurrencyService> _logger)
        {
            _client = client;
            _logger.LogInformation("ForeignCurrencyService initialized with base address: {BaseAddress}", _client.BaseAddress);
        }

        public async Task<OneOf<FiscalData<ForeignCurrencyDescription>, InternalErrorResult>> GetAvailableCurrencies()
        {
            try
            {
                return await GetFiscalDataOrErrrorForAvailableCurrencies();
            }
            catch (HttpRequestException e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                    Message = $"Failed to retrieve currency descriptions due to an HTTP request error: {e.Message}"
                };
            }
            catch (JsonException e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Failed to parse currency descriptions due to a JSON error: {e.Message}"
                };
            }
            catch (Exception e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"An unexpected error occurred while retrieving currency descriptions: {e.Message}"
                };
            }
        }

        private async Task<OneOf<FiscalData<ForeignCurrencyDescription>, InternalErrorResult>> GetFiscalDataOrErrrorForAvailableCurrencies()
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
                                FiscalData<ForeignCurrencyDescription>? currencyList = await JsonSerializer.DeserializeAsync<FiscalData<ForeignCurrencyDescription>>(sr);

                                if (currencyList != null)
                                {
                                    return currencyList;
                                }
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)HttpStatusCode.NotFound,
                                    Message = $"No Currency List Found"
                                };
                            }
                        }
                    }
                }

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    using (var sr = await result.Content.ReadAsStreamAsync())
                    {
                        if (sr != null)
                        {
                            FiscalErrorResult? errorResult = await JsonSerializer.DeserializeAsync<FiscalErrorResult>(sr);

                            if (errorResult != null)
                            {
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)result.StatusCode,
                                    Message = $"Failed to retrieve currency descriptions. External Site failed with the message {errorResult.Message}"
                                };
                            }
                            else
                            {
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)result.StatusCode,
                                    Message = $"Failed to retrieve currency descriptions. Status Code: {(int)result.StatusCode} - {result.ReasonPhrase}"
                                };
                            }
                        }
                    }
                }
            }

            return new InternalErrorResult
            {
                StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                Message = "Failed to retrieve currency descriptions due to an unknown error."
            };
        }

        public async Task<OneOf<FiscalData<ExchangeRateItem>, InternalErrorResult>> GetConversionRate(CurrencyConversionRateRequest request)
        {
            var exchangeRateQueryString = $"{_client.BaseAddress}?page[size]=1&sort=-effective_date&fields=effective_date,exchange_rate,country_currency_desc,country,record_date&filter=country_currency_desc:eq:{request.CurrencyIdentifier},effective_date:lte:{request.TransactionDate:yyyy-MM-dd}";

            try
            {
                return await RetrieveCurrencyConversionRate(request, exchangeRateQueryString);
            }
            catch (HttpRequestException e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                    Message = $"Failed to retrieve exchange rate due to an HTTP request error: {e.Message}"
                };
            }
            catch (JsonException e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Failed to parse exchange rate due to a JSON error: {e.Message}"
                };
            }
            catch (Exception e)
            {
                return new InternalErrorResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"An unexpected error occurred while retrieving exchange rate: {e.Message}"
                };
            }
        }

        private async Task<OneOf<FiscalData<ExchangeRateItem>, InternalErrorResult>> RetrieveCurrencyConversionRate(CurrencyConversionRateRequest request, string exchangeRateQueryString)
        {
            using (var result = await _client.GetAsync(exchangeRateQueryString))
            {
                if (result.IsSuccessStatusCode)
                {
                    using (var sr = await result.Content.ReadAsStreamAsync())
                    {
                        if (sr != null)
                        {
                            FiscalData<ExchangeRateItem>? exchangeRateData = JsonSerializer.Deserialize<FiscalData<ExchangeRateItem>>(sr);

                            if (exchangeRateData == null || exchangeRateData?.data.Count() == 0)
                            {
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)HttpStatusCode.NotFound,
                                    Message = $"No exchange rate found for currency '{request.CurrencyIdentifier}' on or before '{request.TransactionDate:yyyy-MM-dd}'."
                                };
                            }
                            if (exchangeRateData != null && exchangeRateData?.data.Count() > 0)
                            {
                                return exchangeRateData;
                            }
                        }
                    }
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return new InternalErrorResult
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = $"No exchange rate found for currency '{request.CurrencyIdentifier}' on or before '{request.TransactionDate:yyyy-MM-dd}'."
                    };
                }
                else
                {
                    using (var sr = await result.Content.ReadAsStreamAsync())
                    {
                        if (sr != null)
                        {
                            FiscalErrorResult? errorResult = await JsonSerializer.DeserializeAsync<FiscalErrorResult>(sr);
                            if (errorResult != null)
                            {
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)result.StatusCode,
                                    Message = $"No exchange rate found for currency '{request.CurrencyIdentifier}' on or before '{request.TransactionDate:yyyy-MM-dd}'."
                                };
                            }
                            else
                            {
                                return new InternalErrorResult
                                {
                                    StatusCode = (int)result.StatusCode,
                                    Message = $"Failed to retrieve exchange rate. Status Code: {(int)result.StatusCode} - {result.ReasonPhrase}"
                                };
                            }
                        }
                    }
                }

                return new FiscalData<ExchangeRateItem>();
            }
        }
    }
}