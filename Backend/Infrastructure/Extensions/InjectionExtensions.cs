using Infrastructure.FileExcel;
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

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IActionRepository, ActionRepository>();
            //services.AddScoped<IGoodsIssueDetailsRepository, GoodsIssueDetailsRepository>();
            //services.AddScoped<IGoodsIssueRepository, GoodsIssueRepository>();
            //services.AddScoped<IGoodsReceiptDetailsRepository, GoodsReceiptDetailsRepository>();
            //services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
            //services.AddScoped<IInventoryRepository, InventoryRepository>();
            //services.AddScoped<IModuleRepository, ModuleRepository>();
            //services.AddScoped<IPermissionRepository, PermissionRepository>();
            //services.AddScoped<IProductRepository, ProductRepository>();
            //services.AddScoped<IRoleRepository, RoleRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
