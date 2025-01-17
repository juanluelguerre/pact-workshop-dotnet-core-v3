using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using Provider.Tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class ProductTest
    {
        private const string PactServiceUri = "http://127.0.0.1:9001";

        private ITestOutputHelper OutputHelper { get; }

        public ProductTest(ITestOutputHelper output)
        {
            this.OutputHelper = output;
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                // NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(this.OutputHelper)
                }
            };

            using var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseUrls(PactServiceUri)
                .Build();

            webHost.Start();


            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);

            var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "..", "pacts",
                "ApiClient-ProductService.json"));

            pactVerifier
                .ServiceProvider("ProductService", new Uri(PactServiceUri))
                .WithFileSource(pactFile)
                .WithProviderStateUrl(new Uri($"{PactServiceUri}/provider-states"))
                .Verify();
        }
    }
}
