using System.Reflection;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using CP4.Application.Interfaces.Repositories;
using CP4.Application.Interfaces.Services;
using CP4.Application.Services;
using CP4.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<ITimeRepository, TimeRepository>();
builder.Services.AddScoped<ITimeService, TimeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();