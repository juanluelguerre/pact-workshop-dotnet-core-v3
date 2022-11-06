using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json;
using provider.Model;
using provider.Repositories;

namespace Provider.Tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IProductRepository repository;
        private readonly IDictionary<string, Action> providerStates;

        public ProviderStateMiddleware(RequestDelegate next, IProductRepository repository)
        {
            this.next = next;
            this.repository = repository;
            this.providerStates = new Dictionary<string, Action>
            {
                { "products exist", this.ProductsExist },
                { "product with ID 10 exists", this.Product10Exists },
                { "no products exist", this.NoProductsExist },
                { "product with ID 11 does not exist", this.Product11DoesNotExist },
            };
        }

        private void ProductsExist()
        {
            var products = new List<Product>()
            {
                new(9, "GEM Visa", "CREDIT_CARD", "v2"),
                new(10, "28 Degrees", "CREDIT_CARD", "v1")
            };

            this.repository.SetState(products);
        }

        private void Product10Exists()
        {
            var products = new List<Product>()
            {
                new(10, "28 Degrees", "CREDIT_CARD", "v1")
            };

            this.repository.SetState(products);
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this.next(context);
            }
        }

        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper())
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !String.IsNullOrEmpty(providerState.State))
                {
                    this.providerStates[providerState.State].Invoke();
                }
            }
        }

        private void NoProductsExist()
        {
            this.repository.SetState(new List<Product>());
        }

        private void Product11DoesNotExist()
        {
            this.ProductsExist();
        }
    }
}
