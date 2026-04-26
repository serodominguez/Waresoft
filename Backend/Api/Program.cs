using Web.Api.Extensions;
using Web.Api.Filters;
using Application.Extensions;
using Infrastructure.Extensions;
using Infrastructure.RateLimit;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.
var Cors = "Cors";

builder.Services.AddInjectionInfrastructure(Configuration);
builder.Services.AddInjectionApplication(Configuration);
builder.Services.AddAuthentication(Configuration);
builder.Services.AddScoped<PermissionAuthorizationFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Cors,
        builder =>
        {
            builder.SetIsOriginAllowed(origin => true)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("Content-Disposition");
        });
});

// Bindear configuración
builder.Services.Configure<EndpointRateLimitOptions>(
    builder.Configuration.GetSection("EndpointRateLimit"));

builder.Services.AddMemoryCache();

var app = builder.Build();

var enableSwagger = app.Configuration.GetValue<bool>("EnableSwagger");

// Pipeline.

app.UseRouting();

app.UseCors(Cors);

app.UseMiddleware<EndpointRateLimit>();

app.UseStaticFiles();

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = "swagger"; 
    });
}
;

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapFallbackToController("Index", "Home");

app.Run();

public partial class Program { }