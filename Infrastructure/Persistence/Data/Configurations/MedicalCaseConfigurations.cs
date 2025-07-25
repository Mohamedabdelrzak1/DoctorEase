using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class MedicalCaseConfigurations : IEntityTypeConfiguration<MedicalCase>
    {
        public void Configure(EntityTypeBuilder<MedicalCase> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BeforeImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.AfterImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // إضافة index على Title للبحث السريع
            builder.HasIndex(x => x.Title);
        }
    }
} 