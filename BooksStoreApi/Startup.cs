using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using AutoMapper;
using BooksStore.Api.Logging;
using BooksStore.Api.Mappers;
using BooksStore.Api.Middlewares;
using BooksStore.Api.RequestModels;
using BooksStore.Api.Validators;
using BookStore.Data;
using BookStore.Data.Abstractions;
using BookStore.Service;
using BookStore.Service.Abstraction;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;

namespace BooksStoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), $"/nlog.{env.ToLower()}.config"));
            Configuration = configuration;



            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt => {
                        opt.RequireHttpsMetadata = false;
                        opt.SaveToken = true;
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration.GetSection("Jwt:Issuer").Value,
                            ValidAudience = Configuration.GetSection("Jwt:Audience").Value,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt:SecretKey").Value)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });


            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "Application API",
                    Description = "Application Documentation",
                    Contact = new OpenApiContact { Name = "Author" },
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://en.wikipedia.org/wiki/MIT_License") }
                });
            });


            services.AddHealthChecks()                
                .AddCheck("Database Health Check", new DBHealthCheckProvider(Configuration.GetConnectionString("Default")));
            services.AddAutoMapper(new Type[] { typeof(MappingModels), typeof(BookStore.Service.Mappers.MappingModels)});
            
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddTransient<IDbConnection>(sp => new SqlConnection(Configuration.GetConnectionString("Default")));
            services.AddTransient<IValidator<BooksRequestModel>, BooksRequestValidator>();
            services.AddTransient<IValidator<BooksUpdateRequestModel>, BooksUpdateRequestValidator>();
            services.AddTransient<IBooksService, BooksService>();            
            services.AddTransient<IBooksRepository, BooksRepository>();

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler(logger);

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "BookStore API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/api/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("api/health");
                endpoints.MapControllers();
            });
        }
    }
}
