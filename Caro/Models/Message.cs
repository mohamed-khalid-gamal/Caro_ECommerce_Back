using System.ComponentModel.DataAnnotations;

namespace Caro.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]  
        public string Content { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
    }
}
