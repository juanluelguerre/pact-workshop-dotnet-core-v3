using System;
using System.Threading.Tasks;

namespace Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var apiClient = new ApiClient(new Uri("http://localhost:9001"));

            Console.WriteLine("**Retrieving product list**");
            var response = await apiClient.GetAllProducts();
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(
                $"Response.Code={response.StatusCode}, Response.Body={responseBody}\n\n");

            int productId = 10;
            Console.WriteLine($"**Retrieving product with id={productId}");
            response = await apiClient.GetProduct(productId);
            responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response.Code={response.StatusCode}, Response.Body={responseBody}");

            Console.WriteLine();
            Console.WriteLine("Press ENTER to finish...");
            Console.ReadLine();
        }
    }
}
