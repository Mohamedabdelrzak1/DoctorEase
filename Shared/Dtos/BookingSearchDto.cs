using System;

namespace Shared.DTO
{
    public class BookingSearchDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public string? PatientName { get; set; }
        public string? Phone { get; set; }
        public string? ServiceType { get; set; }
    }
} 