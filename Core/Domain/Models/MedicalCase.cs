using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class MedicalCase : BaseEntity<int>
    {
        
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string BeforeImageUrl { get; set; }
        
        [Required]
        public string AfterImageUrl { get; set; }
        
      
        
    
    }
} 