using CP4.Domain.Entities;
using CP4.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CP4.IntegrationTests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        $"CP4IntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                DbContextOptions<ApplicationContext>>();

            services.RemoveAll<ApplicationContext>();

            services.AddDbContext<ApplicationContext>(
                options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
        });
    }

    protected override IHost CreateHost(
        IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope =
            host.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<ApplicationContext>();

        context.Database.EnsureCreated();

        if (!context.Times.Any())
        {
            context.Times.Add(
                new Time
                {
                    Id = 1,
                    Nome = "FURIA",
                    Jogo = "CS2",
                    Pais = "Brasil",
                    Ranking = 3
                });

            context.SaveChanges();
        }

        return host;
    }
}