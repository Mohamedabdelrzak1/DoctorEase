using AutoMapper;
using AutoMapper.Execution;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class PictureUrlResolver(IConfiguration configuration) : IValueResolver<Article, ArticleDto, string>
    {
        

        public string Resolve(Article source, ArticleDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl)) return string.Empty;
            return $"{configuration["BaseUrl"]}/{source.PictureUrl}";
        }
    }
}
