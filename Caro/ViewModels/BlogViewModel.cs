using Caro.Models;
using System.ComponentModel.DataAnnotations;

namespace Caro.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string ShortDescription { get; set; }

        public string?ExistingImage { get; set; } // New property to hold the Base64 image string

        public List<Comment>? Comments { get; set; } = new List<Comment>();
    }
}
