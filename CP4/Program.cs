using System.Reflection;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using CP4.Application.Interfaces.Repositories;
using CP4.Application.Interfaces.Services;
using CP4.Application.Services;
using CP4.Infrastructure.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ArenaSync API",
        Version = "v1",
        Description = "API REST para gerenciamento de times, jogadores e perfis competitivos de e-sports."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("OracleConnection")
    )
);

builder.Services.AddHealthChecks()
    .AddCheck(
        "api",
        () => HealthCheckResult.Healthy("API disponível"))
    .AddDbContextCheck<ApplicationContext>(
        name: "oracle-database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "database" });


builder.Services.AddScoped<ITimeRepository, TimeRepository>();
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IJogadorRepository, JogadorRepository>();
builder.Services.AddScoped<IJogadorService, JogadorService>();
builder.Services.AddScoped<IPerfilCompetitivoRepository, PerfilCompetitivoRepository>();
builder.Services.AddScoped<IPerfilCompetitivoService, PerfilCompetitivoService>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ip =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ip,
                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
            });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                durationMs = entry.Value.Duration.TotalMilliseconds
            }),
            totalDurationMs = report.TotalDuration.TotalMilliseconds
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
});

app.Run();