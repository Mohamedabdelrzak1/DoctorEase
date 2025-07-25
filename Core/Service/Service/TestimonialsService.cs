using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using ServiceAbstraction;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class TestimonialsService(IUnitOfWork unitOfWork, IMapper mapper) :ITestimonialsService
    {
        
        public async Task<IEnumerable<TestimonialDto>> GetAllTestimonialsAsync()
        {
            var testimonials = await unitOfWork.GetRepository<Testimonial, int>().GetAllAsync();
            return mapper.Map<IEnumerable<TestimonialDto>>(testimonials);
        }

        public async Task<TestimonialDto?> GetTestimonialByIdAsync(int id)
        {
            var testimonial = await unitOfWork.GetRepository<Testimonial, int>().GetByIdAsync(id);
            return testimonial == null ? null : mapper.Map<TestimonialDto>(testimonial);
        }

        public async Task AddTestimonialAsync(TestimonialDto testimonialDto)
        {
            var testimonial = mapper.Map<Testimonial>(testimonialDto);

            // إذا كان PatientId موجود، تحقق من وجود المريض
            if (testimonialDto.PatientId.HasValue)
            {
                var patient = await unitOfWork.GetRepository<Patient, int>().GetByIdAsync(testimonialDto.PatientId.Value);
                if (patient == null)
                    throw new Exception("Patient not found.");

                testimonial.PatientId = testimonialDto.PatientId.Value;
            }

            await unitOfWork.GetRepository<Testimonial, int>().AddAsync(testimonial);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTestimonialAsync(int id)
        {
            var testimonial = await unitOfWork.GetRepository<Testimonial, int>().GetByIdAsync(id);
            if (testimonial != null)
            {
                unitOfWork.GetRepository<Testimonial, int>().Delete(testimonial);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> CountTestimonialsAsync()
        {
            var testimonials = await unitOfWork.GetRepository<Testimonial, int>().GetAllAsync();
            return testimonials.Count();
        }

        public async Task<double> GetAverageRatingAsync()
        {
            var testimonials = await unitOfWork.GetRepository<Testimonial, int>().GetAllAsync();
            return testimonials.Any() ? Math.Round(testimonials.Average(t => t.Rating), 1) : 0;
        }


    }
}
