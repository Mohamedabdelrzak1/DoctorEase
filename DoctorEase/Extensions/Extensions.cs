using DoctorEase.MiddeelWare;
using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Service;
using Shared.ErrorModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Shared.Dtos.Auth;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DoctorEase.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddBuiltInService(configuration);
            services.AddSwaggerService();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "DoctorEase API", Version = "v1" });

                
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                });

            
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });




            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();


            services.ConfigurService();





            return services;
        }

        private static IServiceCollection AddBuiltInService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });


            // تحميل JwtOptions بشكل صحيح
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = System.Text.Json.JsonSerializer.Serialize(new { message = "🚫 Unauthorized: Invalid or missing token." });
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            return services;
        }



        private static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();



            return services;
        }
        private static IServiceCollection ConfigurService(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(config =>
            {

                config.InvalidModelStateResponseFactory = (actionContext) =>
                {

                    var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                                                 .Select(m => new ValidaionError()
                                                 {
                                                     Field = m.Key,
                                                     Errors = m.Value.Errors.Select(errors => errors.ErrorMessage)
                                                 });

                    var response = new ValidaionErrorResponse()
                    {
                        Errors = errors

                    };

                    return new BadRequestObjectResult(response);
                };


            });


            return services;
        }

        public static async Task<WebApplication> ConfigurMiddelwares(this WebApplication app)
        {

            await app.IntialiseDatabaseAsync();


            // Configure the HTTP request pipeline.

            app.UseGlobalErrorHandling();

            app.UseSwagger();
            app.UseSwaggerUI();


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }
                await next();
            });

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            return app;
        }


        private static async Task<WebApplication> IntialiseDatabaseAsync(this WebApplication app)
        {

            //DataSeeding 
            using var Scoope = app.Services.CreateScope();

            var ObjectOfDataSeeding = Scoope.ServiceProvider.GetRequiredService<IDataSeeding>();

            await ObjectOfDataSeeding.DataSeedAsync();


            return app;
        }

        private static WebApplication UseGlobalErrorHandling(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleWare>();

            return app;
        }
    }
}
