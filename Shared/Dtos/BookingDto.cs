using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class BookingDto
    {
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? Phone { get; set; }
        public DateTime Date { get; set; }
        public string? Time { get; set; }
        public string? Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? DiseaseDescription { get; set; } // وصف المرض أو الحالة
        public string? ServiceType { get; set; } // نوع الخدمة (عيادة، أونلاين، متابعة)
        public string? Email { get; set; } // إيميل المريض
        public string? Address { get; set; } // عنوان المريض
    }
}
