using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Article : BaseEntity<int>
    {
        [Required]
        public string Title { get; set; } = null!; 
        public string Category { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        

    }
}
