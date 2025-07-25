using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.Dtos.ErrorModels;
using Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuditLogController(IServiceManager _serviceManager) : ControllerBase
    {
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AuditLogDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            var logs = await _serviceManager.AuditLogService.GetAuditLogsAsync(null, null, null, null, null);
            return Ok(logs);
        }

        
        [HttpGet("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> TestAuditLogs()
        {
            var logs = await _serviceManager.AuditLogService.GetAuditLogsAsync(null, null, null, null, null);

            return Ok(new
            {
                Count = logs.Count(),
                Message = $"✅ Found {logs.Count()} audit logs.",
                HasData = logs.Any()
            });
        }

        
        [HttpGet("by-phone")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AuditLogDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAuditLogsByPhone([FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return BadRequest(new ErrorDetails
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = "Phone number is required."
                });
            }

            var cleanPhone = phone.Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");

            var logs = await _serviceManager.AuditLogService.GetAuditLogsAsync(null, null, null, null, null);

            var filteredLogs = logs
                .Where(l =>
                    (!string.IsNullOrEmpty(l.ChangedBy) &&
                        l.ChangedBy.Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "") == cleanPhone) ||
                    (!string.IsNullOrEmpty(l.Changes) &&
                        l.Changes.Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "") == cleanPhone)
                )
                .OrderByDescending(l => l.ChangedAt)
                .ToList();

            if (!filteredLogs.Any())
            {
                return NotFound(new ErrorDetails
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = $"No audit logs found for the phone number: {phone}."
                });
            }

            return Ok(filteredLogs);
        }
    }
}
