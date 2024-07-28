namespace Caro.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        // Navigation property for the related Product
        public Product Product { get; set; }

        public string ImageUrl { get; set; }
        public string AltText { get; set; } // Optional, for SEO and accessibility
    }

}
