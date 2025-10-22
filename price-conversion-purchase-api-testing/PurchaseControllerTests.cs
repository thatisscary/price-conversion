namespace price_conversion_testing
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using Azure;
    using price_conversion_data_api.Controllers;
    using price_conversion_purchasedb.Entities;

    public class PurchaseControllerTests
    {
        // Instructions:
        // 1. Add a project reference to the target AppHost project, e.g.:
        //
        //    <ItemGroup>
        //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
        //    </ItemGroup>
        //
        // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' in GlobalSetup.cs to match your AppHost project:
        //
        [ClassDataSource<Data.HttpClientDataClass>]
        [Test]
        public async Task GivenAValidPurchaseRequest_WhenCreating_ThenReturnsCreated(Data.HttpClientDataClass httpClientData)
        {
            // Arrange
            var httpClient = httpClientData.HttpClient;
            PurchaseRequest purchaseRequest = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase"
            };


            // Act
            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest), System.Text.Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync("api/purchases", content, CancellationToken.None);
            // Assert

            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
            
            Assert.Equals(HttpStatusCode.Created, response.StatusCode);
            Assert.Equals("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            await Assert.That(response.Headers.Location).IsNotNull();

        }

        [ClassDataSource<Data.HttpClientDataClass>]
        [Test]
        public async Task GivenARequestForPurchases_WhenLocationIsSet_PurchaseCanBeRetrieved(Data.HttpClientDataClass httpClientData)
        {
            // Arrange
            var httpClient = httpClientData.HttpClient;
            var cancellationToken = CancellationToken.None;
            PurchaseRequest purchaseRequest = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase"
            };

            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest), System.Text.Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync("api/purchases", content, cancellationToken);

            HttpClient getClient = new HttpClient();

            var getResponse = await getClient.GetAsync(response.Headers?.Location?.ToString(), cancellationToken);

            var responseBody = await getResponse.Content.ReadAsStringAsync(cancellationToken);

            var purchaseResult = System.Text.Json.JsonSerializer.Deserialize<Purchase>(responseBody, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            var guidId = response.Headers?.Location?.Segments?.Last();
            Assert.Equals(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equals("application/json; charset=utf-8", getResponse.Content.Headers.ContentType?.ToString());
            await Assert.That(purchaseResult).IsNotNull();
            Assert.Equals(purchaseRequest.totalAmount, purchaseResult?.TotalAmount);
            Assert.Equals(purchaseRequest.transactionDate.ToString("yyyy-MM-dd"), purchaseResult?.TransactionDate.ToString("yyyy-MM-dd"));
            Assert.Equals(purchaseRequest.description, purchaseResult?.Description);
            Assert.Equals(purchaseResult?.PurchaseId.ToString(), guidId);
        }


        [ClassDataSource<Data.HttpClientDataClass>]
        [Test]
        public async Task GivenSavedPurchases_WhenRetrieveListIsSet_ThenAllPurchaseCanBeRetrieved(Data.HttpClientDataClass httpClientData)
        {
            // Arrange
            var httpClient = httpClientData.HttpClient;
            var cancellationToken = CancellationToken.None;
            
            using var response = await httpClient.GetAsync("api/purchases", cancellationToken);
            

            PurchaseRequest purchaseRequest1 = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase-1"
            };

            PurchaseRequest purchaseRequest2 = new PurchaseRequest
            {
                totalAmount = 100.00M,
                transactionDate = DateTime.UtcNow,
                description = "Test Purchase-2"
            };

            HttpContent content1 = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest1), System.Text.Encoding.UTF8, "application/json");

            HttpContent content2 = new StringContent(System.Text.Json.JsonSerializer.Serialize(purchaseRequest2), System.Text.Encoding.UTF8, "application/json");

            await httpClient.PostAsync("api/purchases", content1, cancellationToken);

            await httpClient.PostAsync("api/purchases", content2, cancellationToken);

            await httpClient.GetAsync("api/purchases", cancellationToken);


            // Act
            var getResponse = await httpClient.GetAsync("api/purchases", cancellationToken);


            var responseBody = await getResponse.Content.ReadAsStringAsync(cancellationToken);

            var purcahseList = System.Text.Json.JsonSerializer.Deserialize<List<Purchase>>(responseBody, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.Equals(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equals("application/json; charset=utf-8", getResponse.Content.Headers.ContentType?.ToString());
            await Assert.That(purcahseList).IsNotNull();
            await Assert.That(purcahseList?.Count ?? 0).IsGreaterThanOrEqualTo(2);
            await Assert.That(purcahseList).Contains( p => p.Description == purchaseRequest1.description);
            await Assert.That(purcahseList).Contains(p => p.Description == purchaseRequest2.description);
        }
    }
}
