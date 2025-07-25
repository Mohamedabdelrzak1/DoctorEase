using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class AdminConfigurations : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.Property(a => a.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(a => a.Email).IsUnique();
            builder.Property(a => a.PasswordHash).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
        }
    }
} 