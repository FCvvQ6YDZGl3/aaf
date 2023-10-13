using System.Collections.Generic;
namespace LanguageFeatures.Models
{
    public class Product
    {
        public Product(bool stock = true)
        {
            InStock = stock;
        }
        public string Name { get; set; }
        public string Category { get; set; } = "Watersports";
        public decimal? Price { get; set; }
        public Product Related { get; set; }
        public bool InStock { get; }
        public bool NameBeginWithS => Name?[0] == 'S';
        public static Product[] GetProducts()
        {
            Product[] products = new Product[]
            {
                new Product
                {
                    Name = "Kayak",
                    Category = "Water Craft",
                    Price = 275M
                },
                new Product(false)
                {
                    Name = "Lifejacket",
                    Price = 48.95M
                }
            };
            return products;
        }
    }
}