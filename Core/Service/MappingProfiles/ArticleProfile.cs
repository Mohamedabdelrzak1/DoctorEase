using AutoMapper;
using Domain.Models;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<PictureUrlResolver>()); 

            CreateMap<ArticleDto, Article>();
        }
    }
} 