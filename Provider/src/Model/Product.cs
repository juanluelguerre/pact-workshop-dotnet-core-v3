using System;

namespace provider.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }

        public Product(int id, string name, string type, string version)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Version = version;
        }
    }
}
