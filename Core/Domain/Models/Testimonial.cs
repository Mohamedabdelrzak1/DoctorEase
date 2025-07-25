using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Testimonial : BaseEntity<int>
    {
        public int Rating { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? BookingId { get; set; } // ربط التقييم بالحجز
    }
}

