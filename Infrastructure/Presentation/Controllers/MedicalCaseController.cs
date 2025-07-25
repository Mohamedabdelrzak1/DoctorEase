using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.Dtos;
using Shared.Dtos.ErrorModels;
using System.Security.Claims;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class MedicalCaseController : ControllerBase
    {
        private readonly IMedicalCaseService _medicalCaseService;

        public MedicalCaseController(IMedicalCaseService medicalCaseService)
        {
            _medicalCaseService = medicalCaseService;
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MedicalCaseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateMedicalCase([FromForm] CreateMedicalCaseDto dto)
        {
            if (dto.BeforeImage == null || dto.BeforeImage.Length == 0)
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "📸 Before image is required." });

            if (dto.AfterImage == null || dto.AfterImage.Length == 0)
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = "📸 After image is required." });

            try
            {
                var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
                var result = await _medicalCaseService.CreateMedicalCaseAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorDetails { StatusCode = 400, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = "❌ An error occurred while creating the medical case." });
            }
        }



        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MedicalCaseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateMedicalCase(int id, [FromForm] UpdateMedicalCaseDto dto)
        {
            try
            {
                var result = await _medicalCaseService.UpdateMedicalCaseAsync(id, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = "❌ An error occurred while updating the medical case." });
            }
        }

        
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MedicalCaseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetMedicalCase(int id)
        {
            try
            {
                var result = await _medicalCaseService.GetMedicalCaseByIdAsync(id);
                if (result == null)
                    return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "⚠️ Medical case not found." });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = "❌ An error occurred while retrieving the medical case." });
            }
        }

        
        [HttpGet("all")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MedicalCaseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllMedicalCases()
        {
            try
            {
                var result = await _medicalCaseService.GetAllMedicalCasesAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = "❌ An error occurred while retrieving medical cases." });
            }
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteMedicalCase(int id)
        {
            try
            {
                var result = await _medicalCaseService.DeleteMedicalCaseAsync(id);
                if (!result)
                    return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "⚠️ Medical case not found." });

                return Ok(new { message = "✅ Medical case deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorDetails { StatusCode = 500, ErrorMessage = "❌ An error occurred while deleting the medical case." });
            }
        }



    }
}
