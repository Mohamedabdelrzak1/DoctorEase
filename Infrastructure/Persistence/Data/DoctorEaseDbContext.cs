using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data
{
   public class DoctorEaseDbContext : DbContext
    {


        public DoctorEaseDbContext(DbContextOptions<DoctorEaseDbContext> options) :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);

            base.OnModelCreating(modelBuilder);
        }

      
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<MedicalCase> MedicalCases { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }


    }
}
