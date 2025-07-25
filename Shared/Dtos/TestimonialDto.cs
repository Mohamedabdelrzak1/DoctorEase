using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class TestimonialDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? PatientId { get; set; }
        public string? PatientName { get; set; }
    }
} 