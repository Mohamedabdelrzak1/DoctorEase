using System;

namespace Domain.Models
{
    public class AuditLog : BaseEntity<int>
    {
        public string EntityName { get; set; } = null!; // Booking, Patient
        public int EntityId { get; set; } // رقم الكيان
        public string Action { get; set; } = null!; // Created, Updated, Deleted
        public string? ChangedBy { get; set; } // رقم الجوال أو اسم الأدمن
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        public string? Changes { get; set; } // تفاصيل التغيير (اختياري)
    }
} 