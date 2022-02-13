using System.Collections.Generic;

namespace ECommerce.Shared
{
    public class ProductSearchResult
    {
        public List<Product> Products { get; set; } = new();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}