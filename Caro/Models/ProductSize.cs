using System.ComponentModel.DataAnnotations;

namespace Caro.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        // Navigation property for the related Product
        public Product Product { get; set; }

        public string Size { get; set; } // e.g., Small, Medium, Large, etc.
        public int Quantity { get; set; }

    }
}
