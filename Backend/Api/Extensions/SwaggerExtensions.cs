using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var openApi = new OpenApiInfo
            {
                Title = "Warehouse API",
                Version = "v1",
                Description = "Warehouse System",
                Contact = new OpenApiContact
                {
                    Name = "IT SOLUTIONS",
                    Email = "sergio_sistemas@outlook.com"
                }
            };

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", openApi);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "JWT Bearer Token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                x.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(
                            JwtBearerDefaults.AuthenticationScheme,
                            doc,
                            null
                        ),
                        []
                    }
                });
            });

            return services;
        }
    }
}