using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO
{
    public class UpdateBookingAndPatientDto
    {
        
        public string? PatientName { get; set; }
        [Phone]
        [RegularExpression(@"^(\+?2?0?1[0125][0-9]{8}|(\+?966|0)?5[0-9]{8}|(\+?971|0)?5[0-9]{8}|(\+?962|0)?7[789][0-9]{7}|(\+?963|0)?9[0-9]{8}|(\+?965|0)?[569][0-9]{7}|(\+?964|0)?7[0-9]{9}|(\+?218|0)?9[1-9][0-9]{7}|(\+?973|0)?3[0-9]{7}|(\+?974|0)?3[0-9]{7}|(\+?968|0)?9[0-9]{7}|(\+?20|0)?1[0125][0-9]{8})$", ErrorMessage = "رقم الجوال غير صحيح أو غير مدعوم.")]
        public string? Phone { get; set; }
       
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }

        // بيانات الحجز
        public DateTime? Date { get; set; }
        public string? Time { get; set; }
        public string? DiseaseDescription { get; set; }
        public string? ServiceType { get; set; }
    }
} 