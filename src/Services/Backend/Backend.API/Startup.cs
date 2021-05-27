using Backend.API.Infrastructure;
using Backend.API.Infrastructure.Repositories;
using Backend.API.Infrastructure.Services;
using Backend.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;

namespace Backend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddSwagger(Configuration)
                .AddCustomDbContext(Configuration)
                .AddIntegrationServices(Configuration)
                .AddControllers();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend.API", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BackendContext backendContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.AddApiKeyValidation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Apply the migration
            backendContext.Database.Migrate();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Escape room 2021 - Backend HTTP API",
                    Version = "v1",
                    Description = "The Backend Microservice HTTP API."
                });
            });

            return services;

        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<BackendContext>(options =>
                {
                    options.UseSqlServer(configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                            // Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                });

            return services;
        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddSingleton<IApiKeyService, ApiKeyService>();

            return services;
        }

        public static IApplicationBuilder AddApiKeyValidation(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiKeyValidatorsMiddleware>();

            return app;
        }
    }
}
