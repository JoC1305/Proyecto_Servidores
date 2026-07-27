using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infraestructura.Persistencia;

namespace Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remover registración previa del DbContext
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ContextoDb>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(ContextoDb)).ToList();

            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            // 2. Crear un ServiceProvider interno aislado para EF Core InMemory
            var internalServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // 3. Registrar ContextoDb usando la BD en memoria y el service provider aislado
            services.AddDbContext<ContextoDb>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
                options.UseInternalServiceProvider(internalServiceProvider);
            });
        });
    }
}