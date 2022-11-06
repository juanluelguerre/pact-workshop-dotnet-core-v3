using System.Collections.Generic;
using provider.Model;

namespace provider.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> State { get; set; }

        public ProductRepository()
        {
            this.State = new List<Product>()
            {
                new(9, "GEM Visa", "CREDIT_CARD", "v2"),
                new(10, "28 Degrees", "CREDIT_CARD", "v1")
            };
        }

        public void SetState(List<Product> state)
        {
            this.State = state;
        }

        List<Product> IProductRepository.List()
        {
            return this.State;
        }

        public Product Get(int id)
        {
            return this.State.Find(p => p.Id == id);
        }
    }
}
