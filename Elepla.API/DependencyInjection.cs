using AutoMapper;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Elepla.Repository.Repositories;
using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Mappers;
using Elepla.Service.Services;
using Elepla.Service.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Elepla.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services, JWTSettings jwt)
        {
            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.JWTSecretKey))
                };
            });

            // Add services to the container.
            services.AddHttpContextAccessor();
            services.AddTransient<SeedData>();
            services.AddControllers();
            services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<IFacebookService, FacebookService>();
            services.AddScoped<ISmsSender, TwilioSmsSender>();
            services.AddScoped<IFirebaseService, FirebaseService>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));

            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);

            return services;
        }
    }
}
