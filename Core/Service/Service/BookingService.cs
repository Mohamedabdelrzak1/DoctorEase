using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using ServiceAbstraction;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Helpers;
using Shared.Dtos;

namespace Service.Service
{
    public class BookingService(IUnitOfWork unitOfWork, IMapper mapper, IMailingService mailingService) : IBookingService
    {
        #region Helper Methods

        private static string NormalizePhone(string phone) =>
            phone.Replace(" ", "")
                 .Replace("+", "")
                 .Replace("-", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Trim();

        #endregion

        #region Bookings

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await unitOfWork.GetRepository<Booking, int>().GetByIdAsync(id);
            return booking == null ? null : mapper.Map<BookingDto>(booking);
        }

        public async Task AddBookingAsync(BookingDto bookingDto)
        {
            // منع الحجز في تاريخ قديم
            if (bookingDto.Date.Date < DateTime.Today)
                throw new Exception("لا يمكن الحجز في تاريخ سابق.");

            if (bookingDto.PatientId <= 0)
                throw new Exception("⚠️ Patient ID is required.");

            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(bookingDto.PatientId);
            if (patient == null)
                throw new Exception("⚠️ Patient not found. Please add the patient first.");

            var booking = mapper.Map<Booking>(bookingDto);
            await unitOfWork.GetRepository<Booking, int>().AddAsync(booking);
            await unitOfWork.SaveChangesAsync();

            // إرسال بريد تأكيد الحجز
            if (!string.IsNullOrEmpty(patient.Email) && patient.Email != "unknown@unknown.com")
            {
                var email = new Email
                {
                    To = patient.Email,
                    Subject = "تأكيد حجزك لدى DoctorEase",
                    Body = $@"عزيزي/عزيزتي {patient.Name},\n\nنود إعلامك بأنه تم تسجيل حجزك بنجاح في عيادتنا.\n\nتفاصيل الحجز:\n- التاريخ: {booking.Date:yyyy-MM-dd}\n- الوقت: {booking.Time}\n\nسنقوم بمراجعة الحجز والتواصل معك في حال وجود أي استفسار.\n\nمع أطيب التحيات,\nفريق DoctorEase"
                };
                mailingService.SendEmail(email);
            }
        }

        public async Task AddBookingWithPatientAsync(CreateBookingDto createBookingDto)
        {
            // منع الحجز في تاريخ قديم
            if (createBookingDto.Date.Date < DateTime.Today)
                throw new Exception("لا يمكن الحجز في تاريخ سابق.");

            // تحقق من صحة البيانات الأساسية
            if (string.IsNullOrWhiteSpace(createBookingDto.PatientName) || createBookingDto.PatientName.Trim().ToLower() == "string")
                throw new Exception("يرجى إدخال اسم المريض بشكل صحيح.");

            if (string.IsNullOrWhiteSpace(createBookingDto.Phone) || createBookingDto.Phone.Trim().ToLower() == "string")
                throw new Exception("يرجى إدخال رقم الجوال بشكل صحيح.");

            if (string.IsNullOrWhiteSpace(createBookingDto.Time) || createBookingDto.Time.Trim().ToLower() == "string")
                throw new Exception("يرجى إدخال الوقت بشكل صحيح.");

            if (string.IsNullOrWhiteSpace(createBookingDto.ServiceType) || createBookingDto.ServiceType.Trim().ToLower() == "string")
                throw new Exception("يرجى اختيار نوع الخدمة بشكل صحيح.");

            try
            {
                // Create or find patient
                var patients = await unitOfWork.GetRepository<Patient, int>().GetAllAsync();
                var patient = patients.FirstOrDefault(p => p.Phone == createBookingDto.Phone);

                // تحقق من تعارض المواعيد
                var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
                if (patient != null && bookings.Any(b => b.PatientId == patient.Id && b.Date.Date == createBookingDto.Date.Date && b.Time == createBookingDto.Time))
                    throw new Exception("لا يمكن الحجز في نفس الوقت مرتين لنفس المريض.");

                if (patient == null)
                {
                    // Create new patient
                    patient = new Patient
                    {
                        Name = createBookingDto.PatientName,
                        Phone = createBookingDto.Phone,
                        Email = string.IsNullOrEmpty(createBookingDto.Email) ? "unknown@unknown.com" : createBookingDto.Email,
                        Address = string.IsNullOrEmpty(createBookingDto.Address) ? null : createBookingDto.Address,
                        CreatedAt = DateTime.Now
                    };
                    await unitOfWork.GetRepository<Patient, int>().AddAsync(patient);
                    await unitOfWork.SaveChangesAsync();
                }
                // لا تحدث بيانات المريض لو كان موجود بالفعل

                // Create booking
                var booking = new Booking
                {
                    PatientId = patient.Id,
                    Date = createBookingDto.Date,
                    Time = createBookingDto.Time,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    DiseaseDescription = createBookingDto.DiseaseDescription,
                    ServiceType = createBookingDto.ServiceType
                };

                await unitOfWork.GetRepository<Booking, int>().AddAsync(booking);
                await unitOfWork.SaveChangesAsync();

                // إرسال بريد تأكيد الحجز
                if (!string.IsNullOrEmpty(patient.Email) && patient.Email != "unknown@unknown.com")
                {
                    var email = new Email
                    {
                        To = patient.Email,
                        Subject = "✅ تأكيد حجزك لدى DoctorEase",
                        Body = $@"
مرحبًا {patient.Name},

🎉 يسرنا إعلامك أنه تم **تسجيل حجزك بنجاح** في عيادة DoctorEase.

🗓 **تفاصيل الحجز:**
- التاريخ: {booking.Date:yyyy-MM-dd}
- الوقت: {booking.Time}

💡 سيتم مراجعة الحجز، وسنتواصل معك إذا كان هناك أي استفسار أو تعديل ضروري.

نتمنى لك دوام الصحة والعافية 🌿.

مع أطيب التحيات،
فريق DoctorEase
"

                    };
                    mailingService.SendEmail(email);
                }
            }
            catch (Exception ex)
            {
                // Logging مبسط للخطأ (يمكن استبداله بـ Logger حقيقي)
                Console.WriteLine($"[Booking Error] {ex.Message}");
                throw;
            }
        }

        public async Task UpdateBookingStatusAsync(int id, string status)
        {
            var booking = await unitOfWork.GetRepository<Booking, int>().GetByIdAsync(id);
            if (booking == null)
                throw new Exception($"Booking with ID {id} not found.");

            booking.Status = status;
            unitOfWork.GetRepository<Booking, int>().Update(booking);
            await unitOfWork.SaveChangesAsync();

            // إرسال بريد عند تغيير الحالة
            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(booking.PatientId);
            if (patient != null && !string.IsNullOrEmpty(patient.Email) && patient.Email != "unknown@unknown.com")
            {
                string subject = "";
                string body = "";
                switch (status?.ToLower())
                {
                    case "confirmed":
                        subject = "✅ تم تأكيد حجزك لدى DoctorEase";

                        body = $@"
مرحبًا {patient.Name}،

🎉 يسرنا إعلامك بأنه تم **تأكيد حجزك بنجاح** لدى DoctorEase.

🗓 **تفاصيل الحجز:**
- التاريخ: {booking.Date:yyyy-MM-dd}
- الوقت: {booking.Time}

📍 نرجو الحضور في الموعد المحدد.

💙 نتمنى لك دوام الصحة والعافية.

مع أطيب التحيات،
فريق DoctorEase
";

                        break;
                    case "cancelled":
                    
                        subject = "⚠️ تم إلغاء حجزك لدى DoctorEase";

                        body = $@"
مرحبًا {patient.Name}،

نأسف لإبلاغك بأنه تم **إلغاء حجزك** بناءً على طلبك أو لظروف خاصة.

إذا كان لديك أي استفسار أو رغبت في إعادة الحجز، لا تتردد في التواصل معنا لإعادة ترتيب الموعد في الوقت الذي يناسبك.

💙 نشكرك على ثقتك بنا.

مع أطيب التحيات،
فريق DoctorEase
";

                        break;
                    case "completed":
                        subject = "🙏 شكرًا لزيارتك DoctorEase";

                        body = $@"
مرحبًا {patient.Name}،

💙 نشكرك على حضورك في الموعد المحدد لدى DoctorEase.

نتمنى لك دوام الصحة والعافية، وإذا كان لديك أي ملاحظات أو استفسارات حول الخدمة المقدمة، لا تتردد في التواصل معنا.

✨ يسعدنا دائمًا خدمتك.

مع أطيب التحيات،
فريق DoctorEase
";

                        break;
                    default:
                        subject = "🔔 تحديث حالة حجزك لدى DoctorEase";

                        body = $@"
مرحبًا {patient.Name}،

تم **تحديث حالة حجزك** إلى:
➡️ {status}

🗓 **تفاصيل الحجز:**
- التاريخ: {booking.Date:yyyy-MM-dd}
- الوقت: {booking.Time}

إذا كان لديك أي استفسار، فريقنا في خدمتك دائمًا.

💙 نتمنى لك الصحة والعافية.

مع أطيب التحيات،
فريق DoctorEase
";

                        break;
                }
                var email = new Email
                {
                    To = patient.Email,
                    Subject = subject,
                    Body = body
                };
                mailingService.SendEmail(email);
            }
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingPatientByIdAsync(int patientId)
        {
            // ✅ تحقق من وجود المريض أولاً
            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(patientId);
            if (patient == null)
                return Enumerable.Empty<BookingDto>();

            // ✅ جلب جميع الحجوزات ثم تصفية الحجوزات الخاصة بالمريض
            var allBookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            var patientBookings = allBookings
                .Where(b => b.PatientId == patientId)
                .ToList();

            // ✅ تحويل إلى DTO قبل الإرجاع
            return mapper.Map<IEnumerable<BookingDto>>(patientBookings);
        }



        public async Task<bool> CancelBookingByPatientAsync(int bookingId, string phone)
        {
            var booking = await unitOfWork.GetRepository<Booking, int>().GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(booking.PatientId);
            if (patient == null || NormalizePhone(patient.Phone) != NormalizePhone(phone))
                return false;

            if (booking.Status == "Cancelled")
                return false;

            booking.Status = "Cancelled";
            unitOfWork.GetRepository<Booking, int>().Update(booking);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBookingAndPatientByPatientAsync(int bookingId, string phone, UpdateBookingAndPatientDto dto)
        {
            var booking = await unitOfWork.GetRepository<Booking, int>().GetByIdAsync(bookingId);
            if (booking == null)
                return false;

            var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(booking.PatientId);
            if (patient == null || NormalizePhone(patient.Phone) != NormalizePhone(phone))
                return false;

            if ((DateTime.UtcNow - booking.CreatedAt.ToUniversalTime()).TotalHours > 3)
                return false;

            if (dto.Date.HasValue && !string.IsNullOrEmpty(dto.Time))
            {
                var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
                if (bookings.Any(b => b.PatientId == patient.Id && b.Id != booking.Id && b.Date.Date == dto.Date.Value.Date && b.Time == dto.Time))
                    return false;
            }

            if (!string.IsNullOrEmpty(dto.PatientName))
                patient.Name = dto.PatientName;
            if (!string.IsNullOrEmpty(dto.Phone))
                patient.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Email))
                patient.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Address))
                patient.Address = dto.Address;
            unitOfWork.GetRepository<Patient, int>().Update(patient);

            if (dto.Date.HasValue)
                booking.Date = dto.Date.Value;
            if (!string.IsNullOrEmpty(dto.Time))
                booking.Time = dto.Time;
            if (!string.IsNullOrEmpty(dto.DiseaseDescription))
                booking.DiseaseDescription = dto.DiseaseDescription;
            if (!string.IsNullOrEmpty(dto.ServiceType))
                booking.ServiceType = dto.ServiceType;
            unitOfWork.GetRepository<Booking, int>().Update(booking);

            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDto>> SearchBookingsAsync(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return Enumerable.Empty<BookingDto>();

            var repo = unitOfWork.GetRepository<Booking, int>();
            var bookings = await repo.GetAllAsync();

            // تطابق كامل
            var exactMatches = bookings.Where(b =>
                b.Id.ToString() == search ||
                (b.Patient != null && (b.Patient.Name == search || b.Patient.Phone == search))
            );

            // بحث جزئي
            var partialMatches = bookings.Where(b =>
                (b.Patient != null && (b.Patient.Name.Contains(search) || b.Patient.Phone.Contains(search))) ||
                b.Id.ToString().Contains(search)
            );

            var results = exactMatches.Union(partialMatches).ToList();
            return mapper.Map<IEnumerable<BookingDto>>(results);
        }

        public async Task<int> CountBookingsAsync()
        {
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return bookings.Count();
        }

        public async Task<int> CountBookingsByStatusAsync(string status)
        {
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return bookings.Count(b => b.Status == status);
        }

        public async Task<int> CountBookingsTodayAsync()
        {
            var today = DateTime.Today;
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return bookings.Count(b => b.CreatedAt.Date == today);
        }

        public async Task<int> CountBookingsThisWeekAsync()
        {
            var weekStart = DateTime.Today.AddDays(-7);
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return bookings.Count(b => b.CreatedAt.Date >= weekStart);
        }

        public async Task<DateTime?> GetLastBookingDateAsync()
        {
            var bookings = await unitOfWork.GetRepository<Booking, int>().GetAllAsync();
            return bookings.Any() ? bookings.Max(b => b.CreatedAt) : (DateTime?)null;
        }


        public async Task<IEnumerable<BookingDto>> GetTodayBookingsAsync()
        {
            var today = DateTime.Today;

            var bookings = await unitOfWork
                .GetRepository<Booking, int>()
                .GetAllAsync();

            var todayBookings = bookings
                .Where(b => b.Date.Date == today)
                .OrderBy(b => b.Time)
                .ToList();

            return mapper.Map<IEnumerable<BookingDto>>(todayBookings);
        }




        #endregion
    }
}
