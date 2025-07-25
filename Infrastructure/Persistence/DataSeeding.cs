using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Security.AccessControl;
using System.Text.Json;

namespace Persistence
{
    public class DataSeeding(DoctorEaseDbContext _dbContext) : IDataSeeding
    {
        public async Task DataSeedAsync()
        {
            // 1️⃣ تحقق من المايجريشن المعلقة
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            try
            {
                // ✅ Apply Pending Migrations if exists
                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }



                // 2️⃣ Seeding Patients
                if (!_dbContext.Patients.Any())
                {
                    // 1. قراءة البيانات من ملف JSON
                    var patientsData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\Patients.json");

                    // 2. تحويل البيانات إلى List<Patient>
                    var patients = await JsonSerializer.DeserializeAsync<List<Patient>>(patientsData);

                    // 3. إضافة البيانات إلى قاعدة البيانات
                    if (patients is not null && patients.Any())
                    {
                        await _dbContext.Patients.AddRangeAsync(patients);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                // 3️⃣ Seeding Articles
                if (!_dbContext.Articles.Any())
                {
                    var articlesData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\Articles.json");
                    var articles = await JsonSerializer.DeserializeAsync<List<Article>>(articlesData);

                    if (articles is not null && articles.Any())
                    {
                        await _dbContext.Articles.AddRangeAsync(articles);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                // 4️⃣ Seeding Testimonials
                if (!_dbContext.Testimonials.Any())
                {
                    var testimonialsData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\Testimonials.json");
                    var testimonials = await JsonSerializer.DeserializeAsync<List<Testimonial>>(testimonialsData);

                    if (testimonials is not null && testimonials.Any())
                    {
                        await _dbContext.Testimonials.AddRangeAsync(testimonials);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                // 5️⃣ Seeding Bookings
                if (!_dbContext.Bookings.Any())
                {
                    var bookingsData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\Bookings.json");
                    var bookings = await JsonSerializer.DeserializeAsync<List<Booking>>(bookingsData);

                    if (bookings is not null && bookings.Any())
                    {
                        await _dbContext.Bookings.AddRangeAsync(bookings);

                    }
                    await _dbContext.SaveChangesAsync();
                }

                // 6️⃣ Seeding AuditLogs

                if (!_dbContext.AuditLogs.Any())
                {
                    var auditLogssData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\AuditLogs.json");
                    var auditLogs = await JsonSerializer.DeserializeAsync<List<AuditLog>>(auditLogssData);

                    if (auditLogs is not null && auditLogs.Any())
                    {
                        await _dbContext.AuditLogs.AddRangeAsync(auditLogs);

                    }
                    await _dbContext.SaveChangesAsync();
                }



                // 7️⃣ Seeding Admins
                if (!_dbContext.Admins.Any())
                {
                    var adminsData = File.OpenRead(@"..\Infrastructure\Persistence\Data\DataSeeding\Admins.json");
                    var admins = await JsonSerializer.DeserializeAsync<List<Admin>>(adminsData);
                    if (admins is not null && admins.Any())
                    {
                        await _dbContext.Admins.AddRangeAsync(admins);
                        
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Seeding Failed: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
            }
        }
    }
}
