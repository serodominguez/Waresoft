using Api.Extensions;
using Api.Filters;
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors(Cors);

app.UseMiddleware<EndpointRateLimit>();

app.UseStaticFiles();

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