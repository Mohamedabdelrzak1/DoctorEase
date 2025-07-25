using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    public class PatientConfigurations : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(100);
            builder.Property(p => p.CreatedAt).IsRequired();

            builder.HasMany(p => p.Bookings)
                   .WithOne(b => b.Patient)
                   .HasForeignKey(b => b.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Testimonials)
                   .WithOne(t => t.Patient)
                   .HasForeignKey(t => t.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
