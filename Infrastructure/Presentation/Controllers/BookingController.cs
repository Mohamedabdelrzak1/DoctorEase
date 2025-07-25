using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.DTO;
using Shared.Dtos.ErrorModels;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BookingController(IServiceManager _serviceManager) : ControllerBase
    {
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            if (createBookingDto is null)
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Booking data is required." });

            if (string.IsNullOrWhiteSpace(createBookingDto.PatientName))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Patient name is required." });

            if (string.IsNullOrWhiteSpace(createBookingDto.Phone))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Patient phone is required." });

            await _serviceManager.BookingService.AddBookingWithPatientAsync(createBookingDto);

            return Ok(new { message = "✅ Booking created successfully." });
        }

        [HttpGet("patient/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetPatientBookings(int id)
        {
            var bookings = await _serviceManager.BookingService.GetAllBookingPatientByIdAsync(id);

            if (bookings is null || !bookings.Any())
                return NotFound(new ErrorDetails
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = $"No bookings found for patient with ID: {id}."
                });

            return Ok(bookings);
        }

        [HttpPut("{id}/update")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateBookingAndPatientByPatient(int id, [FromQuery] string phone, [FromBody] UpdateBookingAndPatientDto dto)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Phone is required." });

            var normalizedPhone = phone.Replace(" ", "").Replace("+", "");

            var result = await _serviceManager.BookingService.UpdateBookingAndPatientByPatientAsync(id, normalizedPhone, dto);

            if (!result)
                return BadRequest(new ErrorDetails
                {
                    StatusCode = 400,
                    ErrorMessage = "Update not allowed: booking does not exist, does not belong to this phone, or the 3-hour update window has expired."
                });

            return Ok(new { message = "✅ Booking and patient details updated successfully." });
        }


    }
}
