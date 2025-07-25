using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ServiceAbstraction;
using Shared.DTO;
using Shared.Dtos.ErrorModels;
using System.Runtime.InteropServices;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestimonialController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public TestimonialController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestimonialDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetTestimonials()
        {
            try
            {
                var testimonials = await _serviceManager.TestimonialsService.GetAllTestimonialsAsync();
                return Ok(testimonials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = ex.Message });
            }
        }

        
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestimonialDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetTestimonialById(int id)
        {
            try
            {
                var result = await _serviceManager.TestimonialsService.GetTestimonialByIdAsync(id);
                if (result == null)
                    return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = $"Testimonial with ID {id} not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateTestimonial([FromBody] TestimonialDto testimonialDto)
        {
            try
            {
                if (testimonialDto == null)
                    return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Testimonial data is required." });

                if (testimonialDto.Rating < 1 || testimonialDto.Rating > 5)
                    return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "Rating must be between 1 and 5." });

                await _serviceManager.TestimonialsService.AddTestimonialAsync(testimonialDto);
                return Ok(new { message = "Testimonial created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = ex.Message });
            }
        }

       
        [HttpDelete("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteTestimonial(int id)
        {
            try
            {
                await _serviceManager.TestimonialsService.DeleteTestimonialAsync(id);
                return Ok(new { message = $"Testimonial {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = ex.Message });
            }
        }
    }
} 