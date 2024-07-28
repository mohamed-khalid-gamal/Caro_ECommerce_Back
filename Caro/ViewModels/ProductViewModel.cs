using System.ComponentModel.DataAnnotations;

namespace Caro.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<ProductSizeViewModel> Sizes { get; set; } = new List<ProductSizeViewModel>();
        public List<ProductImageViewModel> Images { get; set; } = new List<ProductImageViewModel>();

        [Required(ErrorMessage = "Please upload at least one image.")]
        [Display(Name = "Upload Images")]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
