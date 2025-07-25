using Microsoft.AspNetCore.Http;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IArticlesService
    {
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task<ArticleDto?> GetArticleByIdAsync(int id);
        Task AddArticleAsync(ArticleDto articleDto, IFormFile? imageFile);
        Task UpdateArticleAsync(int id, ArticleDto articleDto, IFormFile? imageFile);
        Task DeleteArticleAsync(int id);
    }
}
