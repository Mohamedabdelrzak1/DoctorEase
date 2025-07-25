using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Repositorys;
using Service.Service;
using ServiceAbstraction;
using Service.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Persistence
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddDbContext<DoctorEaseDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }); // Allow DI for StoreDbContext 

            var mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
            services.AddSingleton(mailSettings);
            services.AddScoped<IMedicalCaseService, MedicalCaseService>(); // Allow DI for MedicalCaseService 
            services.AddScoped<IDataSeeding, DataSeeding>();   // Allow DI for DataSeeding 
            services.AddScoped<IUnitOfWork, UnitOfWork>();     // Allow DI for UnitOfWork
            


           



            return services;
        }
    }
}
