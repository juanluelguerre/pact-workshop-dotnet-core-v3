using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using provider.Model;
using provider.Repositories;

namespace Provider.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository repository;

        public ProductsController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        // GET /api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = this.repository.List();
            return products;
        }

        // GET /api/products/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = this.repository.Get(id);

            if (product == null)
            {
                return new NotFoundResult();
            }

            return product;
        }
    }
}
