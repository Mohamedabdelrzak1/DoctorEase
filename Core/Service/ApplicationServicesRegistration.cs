using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

                        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AssemblyReference).Assembly)); // Corrected AutoMapper registration
            services.AddScoped<IServiceManager, ServiceManager>(); //Allow DI  for ServiceManager
            services.AddScoped<IMailingService, MailingService>(); //Allow DI for MailingService           

            return services;
        }
    }
}
