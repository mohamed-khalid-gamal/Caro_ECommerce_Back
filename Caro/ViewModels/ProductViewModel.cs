using System.ComponentModel.DataAnnotations;

namespace Caro.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Must be Name ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Must be Description ")]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<ProductSizeViewModel> Sizes { get; set; } = new List<ProductSizeViewModel>();
        public List<ProductImageViewModel> Images { get; set; } = new List<ProductImageViewModel>();
        public string? ImageToRemove { get; set; }

        [Display(Name = "Upload Images")]
        public List<IFormFile>?ImageFiles { get; set; }
    }
}
