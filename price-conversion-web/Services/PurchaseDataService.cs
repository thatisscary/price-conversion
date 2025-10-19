namespace price_conversion_web.Services
{
    using System.Text.Json;
    using price_conversion_web.Models;

    public class PurchaseDataService
    {
        private readonly HttpClient _purchaseApiClient;
        private readonly ILogger _logger;

        public PurchaseDataService(HttpClient purchaseApiClient, ILogger logger)
        {
            _purchaseApiClient = purchaseApiClient;
            _logger = logger;
        }

        public async Task<PurchaseResult[]> GetPurchasesDataAsync()
        {
            try
            {
                using (var result = await _purchaseApiClient.GetAsync(_purchaseApiClient.BaseAddress))
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
                                    PurchaseResult[] purchases = await JsonSerializer.DeserializeAsync<PurchaseResult[]>(sr);

                                    return purchases ?? Array.Empty<PurchaseResult>();
                                }
                            }
                        }
                    }

                    return Array.Empty<PurchaseResult>();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
                return Array.Empty<PurchaseResult>();
            }
        }
    }
}