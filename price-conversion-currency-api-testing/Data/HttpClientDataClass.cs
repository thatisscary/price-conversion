using TUnit.Core.Interfaces;

namespace price_conversion_currency_api_testing.Data
{
    public class HttpClientDataClass : IAsyncInitializer, IAsyncDisposable
    {
        public HttpClient HttpClient { get; private set; } = new();
        public async Task InitializeAsync()
        {
            HttpClient = (GlobalHooks.App ?? throw new NullReferenceException()).CreateHttpClient("currency-conversion-api");
            if (GlobalHooks.NotificationService != null)
            {
                await GlobalHooks.NotificationService.WaitForResourceAsync("currency-conversion-api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(100));
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Console.Out.WriteLineAsync("And when the class is finished with, we can clean up any resources.");
            HttpClient.Dispose();
        }
    }
}
