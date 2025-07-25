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
    public class BookingConfigurations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.Date).IsRequired();
            builder.Property(b => b.Time).HasMaxLength(10);
            builder.Property(b => b.Status).IsRequired().HasMaxLength(50);
            builder.Property(b => b.CreatedAt).IsRequired();

            builder.HasOne(b => b.Patient)
                   .WithMany(p => p.Bookings)
                   .HasForeignKey(b => b.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
