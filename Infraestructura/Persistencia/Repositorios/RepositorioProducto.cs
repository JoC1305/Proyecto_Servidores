using Aplicacion.Abstracciones.Persistencia;
using Dominio.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia.Repositorios;

public sealed class RepositorioProducto : IRepositorioProducto
{
    private readonly ContextoDb contextoDb;

    public RepositorioProducto(ContextoDb contextoDb)
    {
        this.contextoDb = contextoDb;
    }

    public async Task AgregarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await contextoDb.Productos.AddAsync(producto, cancellationToken);
    }

    public Task<Producto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return contextoDb.Productos.FirstOrDefaultAsync(producto => producto.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Producto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await contextoDb.Productos.AsNoTracking().OrderBy(producto => producto.Nombre).ToArrayAsync(cancellationToken);
    }

    public Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default)
    {
        var codigoNormalizado = codigoBarras.Trim();
        return contextoDb.Productos.AnyAsync(producto => producto.CodigoBarras.Valor == codigoNormalizado, cancellationToken);
    }
}
