using System.ComponentModel.DataAnnotations;

namespace Caro.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser Author { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public Comment()
        {
            Date = DateTime.Now;
        }
    }
}
