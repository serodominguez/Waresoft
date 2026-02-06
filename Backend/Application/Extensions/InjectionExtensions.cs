using Application.Commons.Ordering;
using Application.Interfaces;
using Application.Security;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToArray();

            foreach (var assembly in assemblies)
            {
                AssemblyScanner
                    .FindValidatorsInAssembly(assembly)
                    .ForEach(result =>
                    {
                        services.AddScoped(result.InterfaceType, result.ValidatorType);
                    });
            }

            services.AddTransient<IOrderingQuery, OrderingQuery>();
            services.AddTransient<ISecurity, SecurityApplication>();
            services.AddTransient<IGeneratePdfService, GeneratePdfService>();

            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IGenerateExcelService, GenerateExcelService>();
            services.AddScoped<IGoodsIssueService, GoodsIssueService>();
            services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStoreInventoryService, StoreInventoryService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
