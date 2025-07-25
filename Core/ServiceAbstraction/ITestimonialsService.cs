using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ITestimonialsService
    {
        Task<IEnumerable<TestimonialDto>> GetAllTestimonialsAsync();
        Task<TestimonialDto?> GetTestimonialByIdAsync(int id);
        Task AddTestimonialAsync(TestimonialDto testimonialDto);
        Task DeleteTestimonialAsync(int id);

        Task<int> CountTestimonialsAsync();
        Task<double> GetAverageRatingAsync();

    }
}
