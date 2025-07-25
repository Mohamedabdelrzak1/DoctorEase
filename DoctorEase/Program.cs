using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ServiceAbstraction;
using System.Threading.Tasks;
using Persistence.Data;
using Domain.Contracts;
using Persistence;
using Persistence.Repositorys;
using Service;
using DoctorEase.MiddeelWare;
using DoctorEase.Extensions;

namespace DoctorEase
{
    public class Program
    {
        public static async Task Main(string[] args)
        {



            var builder = WebApplication.CreateBuilder(args);

            builder.Services.RegisterAllServices(builder.Configuration);



            var app = builder.Build();

            await app.ConfigurMiddelwares();

            app.Run();

      
        }
    }
}
