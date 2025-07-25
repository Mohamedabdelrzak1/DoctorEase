using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO
{
    public class CreateBookingDto
    {
        
        [Required(ErrorMessage = "اسم المريض مطلوب.")]
        [MinLength(2, ErrorMessage = "اسم المريض قصير جدًا.")]
        [RegularExpression(@"^(?!string$).+", ErrorMessage = "يرجى إدخال اسم المريض بشكل صحيح.")]
        public string PatientName { get; set; }


        [Required(ErrorMessage = "رقم الجوال مطلوب.")]
        [RegularExpression(@"^(\+|00)[1-9][0-9]{7,14}$", ErrorMessage = "يرجى إدخال رقم جوال صحيح يبدأ بكود الدولة.")]
        public string Phone { get; set; }



        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
        public string? Email { get; set; }

        
        public string? Address { get; set; }

        
        [Required(ErrorMessage = "تاريخ الحجز مطلوب.")]
        public DateTime Date { get; set; }

       
        [Required(ErrorMessage = "الوقت مطلوب.")]
        [RegularExpression(@"^(?!string$).+", ErrorMessage = "يرجى إدخال الوقت بشكل صحيح.")]
        public string Time { get; set; }

        [Required(ErrorMessage = "نوع الخدمة مطلوب.")]
        [RegularExpression(@"^(عيادة|أونلاين|متابعة)$", ErrorMessage = "يرجى اختيار نوع خدمة صحيح.")]
        public string ServiceType { get; set; }

        
        public string? DiseaseDescription { get; set; }
    }
}
