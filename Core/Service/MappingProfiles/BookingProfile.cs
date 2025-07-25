using AutoMapper;
using Domain.Models;
using Shared.DTO;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.Phone,
                    opt => opt.MapFrom(src => src.Patient.Phone))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Patient.Email))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.Patient.Address))
                .ForMember(dest => dest.DiseaseDescription, opt => opt.MapFrom(src => src.DiseaseDescription))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType));


            CreateMap<BookingDto, Booking>()
                .ForMember(dest => dest.DiseaseDescription, opt => opt.MapFrom(src => src.DiseaseDescription))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType));
                
            CreateMap<AuditLog, AuditLogDto>();
        }
    }
}



