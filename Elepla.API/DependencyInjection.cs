using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Elepla.Repository.Repositories;
using Elepla.Service.Interfaces;
using Elepla.Service.Services;
using Microsoft.EntityFrameworkCore;

namespace Elepla.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services/*, JWTSettings jwt*/)
        {
            // Add services to the container.

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrentTime, CurrentTime>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));

            return services;
        }
    }
}
