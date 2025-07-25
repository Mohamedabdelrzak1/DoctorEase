using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Service.Helpers;
using ServiceAbstraction;
using Shared.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ArticlesService(IUnitOfWork unitOfWork, IMapper mapper) : IArticlesService
    {
        private const string _folderName = "Articles";

        #region Articles

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            var articles = await unitOfWork.GetRepository<Article, int>().GetAllAsync();
            return mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<ArticleDto?> GetArticleByIdAsync(int id)
        {
            var article = await unitOfWork.GetRepository<Article, int>().GetByIdAsync(id);
            return article == null ? null : mapper.Map<ArticleDto>(article);
        }

        public async Task AddArticleAsync(ArticleDto articleDto, IFormFile? pictureFile)
        {
            var article = mapper.Map<Article>(articleDto);

            if (pictureFile != null)
            {
                var fileName = ImageSettings.UploadImage(pictureFile, _folderName);
                article.PictureUrl = Path.Combine("images", _folderName, fileName).Replace("\\", "/");
            }

            await unitOfWork.GetRepository<Article, int>().AddAsync(article);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateArticleAsync(int id, ArticleDto articleDto, IFormFile? pictureFile)
        {
            var article = await unitOfWork.GetRepository<Article, int>().GetByIdAsync(id);
            if (article != null)
            {
                article.Title = articleDto.Title;
                article.Category = articleDto.Category;
                article.Content = articleDto.Content;

                if (pictureFile != null)
                {
                    // حذف الصورة القديمة إذا كانت موجودة
                    if (!string.IsNullOrEmpty(article.PictureUrl))
                    {
                        var oldFileName = Path.GetFileName(article.PictureUrl);
                        ImageSettings.DeleteImage(oldFileName, _folderName);
                    }

                    // رفع الصورة الجديدة
                    var fileName = ImageSettings.UploadImage(pictureFile, _folderName);
                    article.PictureUrl = Path.Combine("images", _folderName, fileName).Replace("\\", "/");
                }

                unitOfWork.GetRepository<Article, int>().Update(article);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteArticleAsync(int id)
        {
            var article = await unitOfWork.GetRepository<Article, int>().GetByIdAsync(id);
            if (article != null)
            {
                if (!string.IsNullOrEmpty(article.PictureUrl))
                {
                    var fileName = Path.GetFileName(article.PictureUrl);
                    ImageSettings.DeleteImage(fileName, _folderName);
                }

                unitOfWork.GetRepository<Article, int>().Delete(article);
                await unitOfWork.SaveChangesAsync();
            }
        }

        #endregion
    }
}
