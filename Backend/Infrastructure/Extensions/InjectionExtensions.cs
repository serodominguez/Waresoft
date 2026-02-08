using Infrastructure.FileExcel;
using Infrastructure.FilePdf;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DbContextSystem).Assembly.FullName;
            services.AddDbContext<DbContextSystem>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DbConnection"), b => b.MigrationsAssembly(assembly)), ServiceLifetime.Transient);


            ;
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGenerateExcel, GenerateExcel>();
            services.AddTransient<IListPdfGeneratorFactory, ListPdfGeneratorFactory>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IActionRepository, ActionRepository>();

            return services;
        }
    }
}
