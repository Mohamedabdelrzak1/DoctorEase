using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Patient : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; } // عنوان المريض
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    }
}



