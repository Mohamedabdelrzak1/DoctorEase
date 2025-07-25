using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.ErrorModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BuggyController : ControllerBase
    {
        /// <summary>
        /// Returns 404 Not Found for testing Frontend error handling.
        /// </summary>
        [HttpGet("notfound")]
        public IActionResult GetNotFoundRequestError()
        {
            var errorResponse = new ErrorDetails
            {
                StatusCode = StatusCodes.Status404NotFound,
                ErrorMessage = "The requested resource was not found."
            };

            return NotFound(errorResponse);
        }

        /// <summary>
        /// Returns 500 Internal Server Error for testing Frontend error handling.
        /// </summary>
        [HttpGet("servererror")]
        public IActionResult GetServerRequestError()
        {
            try
            {
                object obj = null;
                var res = obj.ToString(); // This will throw NullReferenceException
                return Ok(res);
            }
            catch
            {
                var errorResponse = new ErrorDetails
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = "An internal server error occurred. Please try again later."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Returns 400 Bad Request for testing Frontend error handling.
        /// </summary>
        [HttpGet("badrequest")]
        public IActionResult GetBadRequestError()
        {
            var errorResponse = new ErrorDetails
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorMessage = "Bad request. Please check your input and try again."
            };

            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Returns 401 Unauthorized for testing Frontend error handling.
        /// </summary>
        [HttpGet("unauthorized")]
        public IActionResult GetUnAuthorizedError()
        {
            var errorResponse = new ErrorDetails
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ErrorMessage = "You are not authorized to access this resource."
            };

            return Unauthorized(errorResponse);
        }
    }
}
