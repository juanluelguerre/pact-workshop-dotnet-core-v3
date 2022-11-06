using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class ApiTest
    {
        private const int port = 9000;

        private readonly IPactBuilderV3 pactBuilder;
        private readonly ApiClient apiClient;
        private readonly List<object> products;

        public ApiTest(ITestOutputHelper output)
        {
            this.products = new List<object>()
            {
                new { id = 9, type = "CREDIT_CARD", name = "GEM Visa", version = "v2" },
                new { id = 10, type = "CREDIT_CARD", name = "28 Degrees", version = "v1" }
            };

            var config = new PactConfig
            {
                PactDir = Path.Join("..", "..", "..", "..", "..", "pacts"),
                //LogDir = "pact_logs",
                Outputters = new[] { new XUnitOutput(output) },
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            var pact = Pact.V3("ApiClient", "ProductService", config);

            this.pactBuilder = pact.WithHttpInteractions(port);

            this.apiClient = new ApiClient(new System.Uri($"http://localhost:{port}"));
        }

        [Fact]
        public async void GetAllProducts()
        {
            // Arange
            this.pactBuilder.UponReceiving("A valid request for all products")
                .Given("products exist")
                .WithRequest(HttpMethod.Get, "/api/products")
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new TypeMatcher(this.products));

            await this.pactBuilder.VerifyAsync(async ctx =>
            {
                var response = await this.apiClient.GetAllProducts();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async void GetProduct()
        {
            // Arange
            this.pactBuilder.UponReceiving("A valid request for a product")
                .Given("product with ID 10 exists")
                .WithRequest(HttpMethod.Get, "/api/products/10")
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new TypeMatcher(this.products[1]));

            await this.pactBuilder.VerifyAsync(async ctx =>
            {
                var response = await this.apiClient.GetProduct(10);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }
    }
}
