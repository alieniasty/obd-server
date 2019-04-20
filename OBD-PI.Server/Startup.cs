using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OBD;
using OBDPI.Server;
using OBDPI.Server.Data;
using OBDPI.Server.Data.Dtos;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Data.Repositories;
using OBDPI.Server.Filters;
using OBDPI.Server.Hubs;

namespace OBD_PI.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services
                .AddMvc()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(new ModelValidationFilter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddEntityFrameworkSqlServer();

            services
                .AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                });

            services
                .AddDbContext<ObdPiContext>(options =>
                {
                     options.UseSqlServer(Configuration.GetConnectionString("obd-pi-db"));
                });

            Mapper.Initialize(config =>
            {
                config.CreateMap<UserDto, User>();
                config.CreateMap<VehicleDto, Vehicle>().ReverseMap();
                config.CreateMap<ObdTelemetryDto, ObdTelemetry>().ReverseMap();
            });

            services.AddScoped<TelemetryRepository>();
            services.AddScoped<VehicleRepository>();
            services.AddScoped<TelemetryHub>();
            services.AddTransient<UserManager<User>>();
            services.AddSingleton(Configuration);

            services
                .AddIdentity<User, IdentityRole>(config =>
                {
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<ObdPiContext>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration.GetSection("Security")["JwtIssuer"],
                        ValidAudience = Configuration.GetSection("Security")["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Security")["JwtIssuer"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:8080"));

            app.UseHttpsRedirection();
            app.UseSignalR(routeBuilder =>
            {
                routeBuilder.MapHub<TelemetryHub>("/telemetryHub");
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
