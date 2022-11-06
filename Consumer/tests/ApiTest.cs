using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class ApiTest
    {
        private const int Port = 9000;

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
                Outputters = new IOutput[] { new XUnitOutput(output) },
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            var pact = Pact.V3("ApiClient", "ProductService", config);

            this.pactBuilder = pact.WithHttpInteractions(Port);

            this.apiClient = new ApiClient(new System.Uri($"http://localhost:{Port}"));
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

        [Fact]
        public async void NoProductsExist()
        {
            // Arange
            this.pactBuilder.UponReceiving("A valid request for all products")
                .Given("no products exist")
                .WithRequest(HttpMethod.Get, "/api/products")
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new TypeMatcher(new List<object>()));

            await this.pactBuilder.VerifyAsync(async ctx =>
            {
                var response = await this.apiClient.GetAllProducts();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async void ProductDoesNotExist()
        {
            // Arange
            this.pactBuilder.UponReceiving("A valid request for a product")
                .Given("product with ID 11 does not exist")
                .WithRequest(HttpMethod.Get, "/api/products/11")
                .WillRespond()
                .WithStatus(HttpStatusCode.NotFound);

            await this.pactBuilder.VerifyAsync(async ctx =>
            {
                var response = await this.apiClient.GetProduct(11);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            });
        }
    }
}
