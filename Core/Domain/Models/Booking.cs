using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Booking : BaseEntity<int>
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = null!;
        public string? Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? DiseaseDescription { get; set; } // وصف المرض أو الحالة
        public string? ServiceType { get; set; } // نوع الخدمة (عيادة، أونلاين، متابعة)

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

       

    }
}
