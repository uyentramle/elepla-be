using Elepla.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Elepla.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));

            return services;
        }
    }
}
