using Aplicacion.Abstracciones.Eventos;
using Aplicacion.Abstracciones.Persistencia;
using Infraestructura.Eventos;
using Infraestructura.Persistencia;
using Infraestructura.Persistencia.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructura;

public static class DependencyInjection
{
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AgromarketFCVT");

        services.AddDbContext<ContextoDb>(options =>
        {
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
        services.AddScoped<IRepositorioCategoria, RepositorioCategoria>();
        services.AddScoped<IRepositorioProducto, RepositorioProducto>();
        services.AddScoped<IRepositorioAsientoContable, RepositorioAsientoContable>();
        services.AddScoped<IRepositorioVenta, RepositorioVenta>();
        services.AddScoped<IRepositorioCompra, RepositorioCompra>();
        services.AddScoped<IRepositorioMovimientoInventario, RepositorioMovimientoInventario>();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
