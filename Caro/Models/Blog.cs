namespace Caro.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string ShortDescription { get; set; }

        public string Image { get; set; }

        public DateTime Date { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

    }


}
