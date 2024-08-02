using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Caro.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        // Navigation property for related ProductSizes
        public List<ProductSize> Sizes { get; set; } = new List<ProductSize>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

    }
}
