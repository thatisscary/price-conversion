namespace price_conversion_web.Services
{
    using System.Text.Json;
    using price_conversion_web.Models;

    public class PurchaseDataService
    {
        private readonly HttpClient _purchaseApiClient;
        private readonly ILogger _logger;
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private const string PurchaseApiEndpoint = "api/purchases";

        public PurchaseDataService(HttpClient purchaseApiClient, ILogger<PurchaseDataService> logger)
        {
            _purchaseApiClient = purchaseApiClient;
            _logger = logger;
        }

        public async Task<PurchaseResult[]> GetPurchasesDataAsync()
        {
            try
            {
                using (var result = await _purchaseApiClient.GetAsync(new Uri(_purchaseApiClient.BaseAddress, PurchaseApiEndpoint)))
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
                                    PurchaseResult[] purchases = await JsonSerializer.DeserializeAsync<PurchaseResult[]>(sr, jsonOptions);

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

        public async Task<PurchaseResult?> GetPurchaseByIdAsync(Guid id)
        {
            var getByIdEndpoint = new Uri(_purchaseApiClient.BaseAddress, PurchaseApiEndpoint);
            return await GetPurchaseByUrlAsync($"{getByIdEndpoint}/{id}");
        }

        public async Task<PurchaseResult?> GetPurchaseByUrlAsync(string purchaseUri)
        {
            try
            {
                using (var result = await _purchaseApiClient.GetAsync(purchaseUri))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        using var purchaseStream = await result.Content.ReadAsStreamAsync();
                        if (purchaseStream != null)
                        {
                            using (var sr = await result.Content.ReadAsStreamAsync())
                            {
                                if (sr != null)
                                {
                                    PurchaseResult? purchase = await JsonSerializer.DeserializeAsync<PurchaseResult>(sr, jsonOptions);
                                    return purchase;
                                }
                            }
                        }
                    }
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
                return null;
            }
        }

        public async Task<(bool success, string itemLocation, PurchaseResult purchase)> CreatePurchaseAsync(PurchaseRequest purchase)
        {
            try
            {
                var purchaseJson = new StringContent(JsonSerializer.Serialize(purchase),System.Text.Encoding.UTF8,"application/json");
                using (var result = await _purchaseApiClient.PostAsync(new Uri(_purchaseApiClient.BaseAddress, PurchaseApiEndpoint), purchaseJson))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        using var purchaseStream = await result.Content.ReadAsStreamAsync();
                        if (purchaseStream != null)
                        {
                            using (var sr = await result.Content.ReadAsStreamAsync())
                            {
                                if (sr != null)
                                {
                                    PurchaseResult? purchaseResult = await JsonSerializer.DeserializeAsync<PurchaseResult>(sr, jsonOptions);

                                    return (true, result.Headers?.Location?.ToString() ?? string.Empty, purchaseResult);
                                }
                            }
                        }
                    }
                    return (false, string.Empty, null);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
                return (false, string.Empty, null);
            }
        }
    }
}