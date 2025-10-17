namespace currency_conversion_api_tests.ServiceTests
{
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using currency_conversion_api_tests.TestHelpers;
    using NUnit;
    public class ForeignCurrencyServiceTests

    {
        public ForeignCurrencyServiceTests()
        {
        }

        [Test]
        public async Task GetAvailableCurrencies_WhenDataProvided_ReturnsData()
        {
            // Arrange
            HttpClient client = new HttpClient(new MockHttpMessageHandler());
            client.BaseAddress = new Uri("http://localhost/");

            var sut = new ForeignCurrencyService(client);

            // Act
            var result = await sut.GetAvailableCurrencies();

            // Assert
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.data.Length, Is.EqualTo(3));
            Assert.That(result.meta.count, Is.EqualTo(3));
        }



        public class MockHttpMessageHandler : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var helloWorld = ForeignCurrencyTestHelper.GetCurrencyList();

                var json = JsonSerializer.Serialize(helloWorld);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(json)))
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                return await Task.FromResult(response);
            }
        }
    }
}