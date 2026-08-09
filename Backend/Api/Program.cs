using Application.Commons.Settings;
using Application.Extensions;
using Infrastructure.Extensions;
using Infrastructure.RateLimit;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using Web.Api.Extensions;
using Web.Api.Filters;
using Web.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// ── Serilog ──────────────────────────────────────────────────────────────────
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn { ColumnName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 64 },
        new SqlColumn { ColumnName = "ThreadId",    DataType = SqlDbType.NVarChar, DataLength = 16 },
        new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 512 },
        new SqlColumn { ColumnName = "StackTrace",  DataType = SqlDbType.NVarChar, DataLength = -1 },
    }
};
columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Remove(StandardColumn.MessageTemplate);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "SystemLogs",
            AutoCreateSqlTable = false
        },
        columnOptions: columnOptions
    )
    .CreateLogger();

builder.Host.UseSerilog();
// ─────────────────────────────────────────────────────────────────────────────

// Add services to the container.
var Cors = "Cors";
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));
builder.Services.AddInjectionInfrastructure(Configuration);
builder.Services.AddInjectionApplication(Configuration);
builder.Services.AddAuthentication(Configuration);
builder.Services.AddScoped<PermissionAuthorizationFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});

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

builder.Services.Configure<EndpointRateLimitOptions>(
    builder.Configuration.GetSection("EndpointRateLimit"));

builder.Services.AddMemoryCache();

var app = builder.Build();

var enableSwagger = app.Configuration.GetValue<bool>("EnableSwagger");

// Pipeline.
app.UseRouting();
app.UseCors(Cors);
app.UseMiddleware<ExceptionMiddleware>();
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

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }