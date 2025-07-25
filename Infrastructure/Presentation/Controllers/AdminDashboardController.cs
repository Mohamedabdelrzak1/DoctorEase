using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.DTO;
using Shared.Dtos.ErrorModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Domain.Models;


namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController(IServiceManager _serviceManager) : ControllerBase
    {
        #region Statistics

        [HttpGet("Statistics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                // 🩺 Bookings
                var totalBookings = await _serviceManager.BookingService.CountBookingsAsync();
                var completedBookings = await _serviceManager.BookingService.CountBookingsByStatusAsync("Completed");
                var cancelledBookings = await _serviceManager.BookingService.CountBookingsByStatusAsync("Cancelled");
                var bookingsToday = await _serviceManager.BookingService.CountBookingsTodayAsync();
                var bookingsThisWeek = await _serviceManager.BookingService.CountBookingsThisWeekAsync();
                var lastBookingDate = await _serviceManager.BookingService.GetLastBookingDateAsync();

                // 🧑‍⚕️ Patients
                var totalPatients = await _serviceManager.PatientsService.CountPatientsAsync();
                var newPatientsThisMonth = await _serviceManager.PatientsService.CountNewPatientsThisMonthAsync();

                // 🌟 Testimonials
                var totalTestimonials = await _serviceManager.TestimonialsService.CountTestimonialsAsync();
                var averageRating = await _serviceManager.TestimonialsService.GetAverageRatingAsync();

                // ✅ Determine Clinic Health
                var isClinicHealthy = completedBookings > cancelledBookings && newPatientsThisMonth > 0;

                return Ok(new
                {
                    totalBookings,
                    completedBookings,
                    cancelledBookings,
                    bookingsToday,
                    bookingsThisWeek,
                    lastBookingDate = lastBookingDate?.ToString("yyyy-MM-dd HH:mm"),
                    totalPatients,
                    newPatientsThisMonth,
                    totalTestimonials,
                    averageRating,
                    isClinicHealthy
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = ex.Message
                });
            }
        }


        #endregion

        #region Bookings

        [HttpGet("bookings-today")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetTodayBookings()
        {
            try
            {
                var todayBookings = await _serviceManager.BookingService.GetTodayBookingsAsync();
                return Ok(todayBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDetails
                {
                    StatusCode = 500,
                    ErrorMessage = ex.Message
                });
            }
        }


        [HttpGet("bookings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _serviceManager.BookingService.GetAllBookingsAsync();
            if (bookings == null || !bookings.Any())
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "No bookings found." });

            return Ok(bookings);
        }


        
        [HttpGet("bookings/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _serviceManager.BookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = $"Booking with ID {id} not found." });

            return Ok(booking);
        }

        [HttpGet("bookings/search")]
        public async Task<IActionResult> SearchBookings([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "يرجى إدخال كلمة بحث." });

            var results = await _serviceManager.BookingService.SearchBookingsAsync(search);
            if (!results.Any())
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "لا يوجد نتائج مطابقة." });

            return Ok(results);
        }
       
        [HttpPut("bookings/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Status is required." });

            var allowedStatuses = new[] { "Pending", "Confirmed", "Cancelled" };
            if (!allowedStatuses.Contains(status))
                return BadRequest(new ErrorDetails
                {
                    StatusCode = 400,
                    ErrorMessage = $"Status must be one of: {string.Join(", ", allowedStatuses)}."
                });

            await _serviceManager.BookingService.UpdateBookingStatusAsync(id, status);
            return Ok(new { message = $"Booking {id} status updated to {status}." });
        }

      

        #endregion

        #region Patients

        /// <summary>
        /// Retrieves all patients in the system.
        /// </summary>
        /// <returns>List of all patients.</returns>
        [HttpGet("patients")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PatientDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _serviceManager.PatientsService.GetAllPatientsAsync();
            if (patients == null || !patients.Any())
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "No patients found." });

            return Ok(patients);
        }

        /// <summary>
        /// Retrieves a patient by their ID.
        /// </summary>
        /// <param name="id">Patient ID.</param>
        /// <returns>Patient data.</returns>
        [HttpGet("patients/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _serviceManager.PatientsService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = $"Patient with ID {id} not found." });

            return Ok(patient);
        }

        
        [HttpGet("patients/search")]
        public async Task<IActionResult> SearchPatients([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "يرجى إدخال كلمة بحث." });

            var results = await _serviceManager.PatientsService.SearchPatientsAsync(search);
            if (!results.Any())
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "لا يوجد نتائج مطابقة." });

            return Ok(results);
        }

        #endregion

        #region Articles


        [HttpPost("articles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateArticle([FromForm] ArticleDto articleDto, IFormFile? imageFile)
        {
            if (articleDto == null)
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Article data is required." });

            await _serviceManager.ArticlesService.AddArticleAsync(articleDto, imageFile);
            return Ok(new { message = "✅ Article created successfully." });
        }


        [HttpGet("articles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArticleDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _serviceManager.ArticlesService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("articles/{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _serviceManager.ArticlesService.GetArticleByIdAsync(id);

            if (article == null)
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = $"Article with ID {id} not found." });

            return Ok(article);
        }


        [HttpPut("articles/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateArticle(int id, [FromForm] ArticleDto articleDto, IFormFile? imageFile)
        {
            if (articleDto == null)
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Article data is required." });

            await _serviceManager.ArticlesService.UpdateArticleAsync(id, articleDto, imageFile);
            return Ok(new { message = $"✅ Article {id} updated successfully." });
        }


        [HttpDelete("articles/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _serviceManager.ArticlesService.DeleteArticleAsync(id);
            return Ok(new { message = $"Article {id} deleted successfully." });
        }

        #endregion

        #region Testimonials

        [HttpGet("testimonials")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestimonialDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllTestimonials()
        {
            var testimonials = await _serviceManager.TestimonialsService.GetAllTestimonialsAsync();
            return Ok(testimonials);
        }

        #endregion

       
        
    }
}
