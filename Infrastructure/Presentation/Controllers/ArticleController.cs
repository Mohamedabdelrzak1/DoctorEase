using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.DTO;
using Shared.Dtos.ErrorModels;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    
    public class ArticleController(IServiceManager _serviceManager) : ControllerBase
    {
        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArticleDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _serviceManager.ArticlesService.GetAllArticlesAsync();

            if (articles == null || !articles.Any())
                return NotFound(new ErrorDetails { StatusCode = 404, ErrorMessage = "No articles found." });

            return Ok(articles);
        }

        
        [HttpGet("{id:int}")]
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
    }
}
