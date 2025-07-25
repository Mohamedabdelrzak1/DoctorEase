using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class TestimonialConfigurations : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.Property(t => t.Rating)
                   .IsRequired();

            builder.Property(t => t.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(t => t.CreatedAt)
                   .IsRequired();

            // علاقة Testimonial ↔️ Patient (اختيارية)
            builder.HasOne(t => t.Patient)
                   .WithMany(p => p.Testimonials)
                   .HasForeignKey(t => t.PatientId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);
        }
    }
}
