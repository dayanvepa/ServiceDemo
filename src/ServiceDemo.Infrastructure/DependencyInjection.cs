using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceDemo.Domain.Interfaces;
using ServiceDemo.Infrastructure.Data;
using ServiceDemo.Infrastructure.Repositories;

namespace ServiceDemo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ServiceDemoDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

           
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}